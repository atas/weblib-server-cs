using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebLibServer.DI;
using SixLabors.ImageSharp;
using WebLibServer.Disk;
using WebLibServer.Internal;
using WebLibServer.Photos;
using WebLibServer.Uploaders;

namespace WebLibServer.Videos;

[UsedImplicitly]
public class VideoWithPreviewUploader(
	VideoUploader videoUploader,
	IPhotoFileUploader photoFileUploader,
	IFileUploader fileUploader,
	IFileDeleter fileDeleter,
	IStorageBucket bucket)
	: IDisposable, IAsyncDisposable
{
	private readonly ILogger<VideoWithPreviewUploader> _logger =
		Defaults.LoggerFactory.CreateLogger<VideoWithPreviewUploader>();

	private string _vidFilePath;
	private string _convertedVidFilePath;

	private VideoAnalyzer _videoAnalyzer;


	private List<VideoSpriteGenerator.SpriteResult> _spriteResult;
	private PhotoFileUploaderResult _previewUploadResult;
	private VideoUploader.VideoUploadResult _videoUploadResult;


	public async Task<Result> Run(string vidFilePath)
	{
		_vidFilePath = vidFilePath;

		_videoAnalyzer = new VideoAnalyzer(_vidFilePath);

		await Task.WhenAll(GenerateAndUploadPreview(), UploadVideo()); //GenerateAndUploadSprite(), 

		return new Result
		{
			// SpriteResult = _spriteResult,
			PreviewUploadResult = _previewUploadResult,
			VideoUploadResult = _videoUploadResult
		};
	}

	private async Task GenerateAndUploadPreview()
	{
		_logger.LogInformation("Preparing to generate video preview");

		using var generator = new VideoPreviewGenerator()
		{
			Width = _videoAnalyzer.Vs?.Width,
			Height = _videoAnalyzer.Vs?.Height,
		};

		var generatedPreviewLocalPath = await generator.Generate(_vidFilePath, 0);

		_previewUploadResult = await photoFileUploader.SetSource(generatedPreviewLocalPath).Upload();
	}

	private async Task GenerateAndUploadSprite()
	{
		_logger.LogInformation("Running Sprite Generator");
		using var spriteGenerator = new VideoSpriteGenerator(_vidFilePath, _videoAnalyzer);

		_spriteResult = await spriteGenerator.Generate();
		var spriteUploadTasks = new List<Task>();

		foreach (var sprite in _spriteResult)
		{
			spriteUploadTasks.Add(Task.Run(async () =>
			{
				using var memoryStream = new MemoryStream();
				await sprite.Image.SaveAsWebpAsync(memoryStream);
				await fileUploader.Upload(memoryStream, sprite.Guid + ".webp", "image/webp", bucket.Sprites);
			}));
		}

		await Task.WhenAll(spriteUploadTasks.ToArray());
	}

	private async Task UploadVideo()
	{
		_logger.LogInformation("Preparing to upload video");
		ConvertIfNeeded();
		_videoUploadResult = await videoUploader.Upload(_convertedVidFilePath ?? _vidFilePath, _videoAnalyzer);
	}

	private void ConvertIfNeeded()
	{
		var resolution = _videoAnalyzer.GetResolution();

		return;

		/*if (_videoAnalyzer.Vs.CodecName == "h264")// && resolution.Width <= 1920 && resolution.Height <= 1080)
		{
			_logger.LogInformation("Won't convert the video because it's already in correct format");
			return;
		}

		_logger.LogInformation("Starting to convert the video to h264 from {Codec}", _videoAnalyzer.Vs.CodecName);

		_convertedVidFilePath = new VideoConverter(_vidFilePath, _videoAnalyzer).Run();*/
	}

	public async Task Revert()
	{
		_logger.LogWarning("Reverting VideoMediaAddFlow");
		foreach (var sprite in _spriteResult ?? new List<VideoSpriteGenerator.SpriteResult>())
		{
			await fileDeleter.Delete(bucket.Sprites, sprite.Guid + ".webp");
		}

		await videoUploader.Revert();
	}

	public class Result
	{
		public List<VideoSpriteGenerator.SpriteResult> SpriteResult { get; init; }

		public PhotoFileUploaderResult PreviewUploadResult { get; init; }
		public VideoUploader.VideoUploadResult VideoUploadResult { get; init; }
	}

	public void Dispose()
	{
		if (_convertedVidFilePath != null && File.Exists(_convertedVidFilePath))
		{
			_logger.LogInformation("Disposing of converted video file {Path}", _vidFilePath);
			File.Delete(_convertedVidFilePath);
		}
	}

	public async ValueTask DisposeAsync()
	{
		Dispose();
	}
}
