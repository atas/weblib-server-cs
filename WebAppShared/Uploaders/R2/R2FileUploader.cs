using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;

namespace WebAppShared.Uploaders.R2;

public class R2FileUploader(R2FileUploaderConfig config, ILogger<R2FileUploader> logger)
	: IFileUploader
{
	public async Task Upload(Stream fileToUpload, string name, string contentType, string bucket)
	{
		logger.LogInformation("Starting to upload {@File} with name {Bucket}/{Name} to R2", fileToUpload, bucket, name);

		using var r2Client =
			new AmazonS3Client(config.Key, config.Secret, new AmazonS3Config()
			{
				ServiceURL = config.ServiceUrl,
				ForcePathStyle = true
			});

		using var fileTransferUtility = new TransferUtility(r2Client);

		// Option 4. Specify advanced settings.
		var fileTransferUtilityRequest = new TransferUtilityUploadRequest
		{
			BucketName = bucket,
			InputStream = fileToUpload,
			// StorageClass = S3StorageClass.Standard,
			PartSize = 1024 * 1024 * 6, // 6 MB.
			Key = name,
			Headers =
			{
				ContentType = contentType, CacheControl = "public, max-age=2592000",
			},
			AutoCloseStream = false,
			DisablePayloadSigning = true,
			AutoResetStreamPosition = true,
		};
		//                fileTransferUtilityRequest.Metadata.Add("param1", "Value1");

		await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

		logger.LogInformation("Upload of file {Bucket}/{Name} to R2 is completed", bucket, name);
	}

	public async Task Upload(string fileToUpload, string name, string contentType, string bucket)
	{
		await using var stream = new FileStream(fileToUpload, FileMode.Open);
		using var memStream = new MemoryStream();
		await stream.CopyToAsync(memStream);
		await Upload(memStream, name, contentType, bucket);
	}
	
	
	public async Task<Stream> Download(string name, string bucket)
	{
		logger.LogInformation("Starting to download file {Bucket}/{Name} from R2", bucket, name);

		using var r2Client =
			new AmazonS3Client(config.Key, config.Secret, new AmazonS3Config()
			{
				ServiceURL = config.ServiceUrl,
				ForcePathStyle = true
			});
		
		try
		{
			using var response = await r2Client.GetObjectAsync(new GetObjectRequest
			{
				BucketName = bucket,
				Key = name
			});

			// Copy the response stream to a memory stream to return it.
			var memoryStream = new MemoryStream();
			await response.ResponseStream.CopyToAsync(memoryStream);

			// Reset the memory stream position to the beginning
			memoryStream.Position = 0;

			logger.LogInformation("Download of file {Bucket}/{Name} from R2 completed", bucket, name);

			// Return the memory stream containing the file
			return memoryStream;
		}
		catch (Exception e)
		{
			logger.LogError(e, "Unknown encountered on server while downloading file {Bucket}/{Name}", bucket, name);
			throw;
		}
	}
}
