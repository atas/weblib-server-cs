using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebAppShared.Uploaders;
using WebAppShared.Utils;

namespace WebAppShared.Videos;

[UsedImplicitly]
public class VideoUploader
{
	private readonly IFileUploader _fileUploader;
	private readonly ILogger _logger;
	private readonly IFileDeleter _fileDeleter;
	private readonly IStorageBucket _bucket;

	private string _uploadedFileName;

	public VideoUploader(IFileUploader fileUploader, ILogger<VideoUploader> logger, IFileDeleter fileDeleter, IStorageBucket bucket)
	{
		_fileUploader = fileUploader;
		_logger = logger;
		_fileDeleter = fileDeleter;
		_bucket = bucket;
	}

	/// <summary>
	/// Returns a video entity but does not save it to the database. This class should not know about the database.
	/// </summary>
	/// <param name="vidFilePath"></param>
	/// <param name="va"></param>
	/// <returns></returns>
	public async Task<VideoUploadResult> Upload(string vidFilePath, VideoAnalyzer va)
	{
		var filename = Guid.NewGuid();

		var resolution = va.GetResolution();

		_uploadedFileName = $"{filename}-{resolution}.mp4";

		await _fileUploader.Upload(vidFilePath, _uploadedFileName, "video/mp4", _bucket.Videos);

		return new VideoUploadResult()
		{
			Id = filename,
			Ext = "mp4",
			Duration = va.Duration.TotalSeconds,
			Format = resolution,
			Height = va.Vs.Height,
			Width = va.Vs.Width,
			SizeInBytes = new FileInfo(vidFilePath).Length,
			Checksum = FileUtils.GetChecksum(vidFilePath)
		};
	}

	public async Task Revert()
	{
		if (_uploadedFileName != null)
		{
			_logger.LogWarning("Reverting uploaded file {FileName}", _uploadedFileName);
			await _fileDeleter.Delete(_bucket.Videos, _uploadedFileName);
		}
	}

	public class VideoUploadResult
	{
		public Guid Id { get; set; }
		public string Ext { get; set; }
		public double Duration { get; set; }
		public VideoResolution Format { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }
		public long SizeInBytes { get; set; }
		public string Checksum { get; set; }
	}
}
