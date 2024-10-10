using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using MoreLinq;

namespace WebAppShared.Uploaders.R2;

public class R2FileDeleter : IFileDeleter
{
	private readonly R2FileUploaderConfig _config;
	private readonly ILogger<R2FileDeleter> _logger;

	public R2FileDeleter(R2FileUploaderConfig config, ILogger<R2FileDeleter> logger)
	{
		_config = config;
		_logger = logger;
	}

	public async Task Delete(string bucket, string fileName)
	{
		await Delete(bucket, new List<string> { fileName });
	}

	public async Task Delete(string bucket, List<string> fileNames)
	{
		using var r2Client =
			new AmazonS3Client(_config.Key, _config.Secret, new AmazonS3Config()
			{
				ServiceURL = _config.ServiceUrl,
				ForcePathStyle = true
			});

		try
		{
			var deleteObjectsRequest = new DeleteObjectsRequest
			{
				BucketName = bucket,
				Objects = fileNames.Select(f => new KeyVersion { Key = f }).ToList()
			};

			_logger.LogInformation("Deleting R2/{Bucket}/{Name}", deleteObjectsRequest.BucketName,
				deleteObjectsRequest.Objects.ToDelimitedString(";"));
			var resp = await r2Client.DeleteObjectsAsync(deleteObjectsRequest);

			if (resp.DeletedObjects.Count != deleteObjectsRequest.Objects.Count)
				throw new Exception(
					$"Could not delete all objects. Requested: {deleteObjectsRequest.Objects.ToDelimitedString(";")}. Deleted: {resp.DeletedObjects.ToDelimitedString(";")}");
		}
		catch (AmazonS3Exception e)
		{
			Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
		}
		catch (Exception e)
		{
			Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
		}
	}
}
