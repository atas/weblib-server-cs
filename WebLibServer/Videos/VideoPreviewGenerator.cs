using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using WebLibServer.Internal;
using WebLibServer.WebSys.DI;
using Size = System.Drawing.Size;

namespace WebLibServer.Videos;

public class VideoPreviewGenerator : IDisposable
{
	private readonly ILogger _logger = Defaults.LoggerFactory.CreateLogger<VideoPreviewGenerator>();

	public int? Width { get; set; }
	public int? Height { get; set; }

	private string _previewPhotoPath;

	public async Task<string> Generate(string vidFilePath, double previewSecond)
	{
		if (_previewPhotoPath != null)
			throw new Exception("Preview photo is already generated. Instantiate a new class.");

		TrySetDimensions(vidFilePath);

		// Facebook OpenGraph shares don't support webp yet...
		var previewPhotoPath = Path.Join(Path.GetTempPath(), Guid.NewGuid() + ".png");

		var size = new Size(Width!.Value, Height!.Value);

		await FFMpeg.SnapshotAsync(vidFilePath, previewPhotoPath, size, TimeSpan.FromSeconds(previewSecond));

		_logger.LogInformation("Generated video preview photo at {Path} from source {SourcePath}", previewPhotoPath,
			vidFilePath);

		_previewPhotoPath = previewPhotoPath;

		return _previewPhotoPath;
	}

	private void TrySetDimensions(string vidFilePath)
	{
		if (Width == null || Height == null)
		{
			var vidAnalyzer = new VideoAnalyzer(vidFilePath);
			(Width, Height) = (vidAnalyzer.Vs.Width, vidAnalyzer.Vs.Height);
		}
	}

	public void Dispose()
	{
		if (_previewPhotoPath != null && File.Exists(_previewPhotoPath))
		{
			_logger.LogInformation("Disposing of generated video preview photo {Path}", _previewPhotoPath);
			File.Delete(_previewPhotoPath);
		}
	}
}
