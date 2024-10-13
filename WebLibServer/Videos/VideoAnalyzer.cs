using System;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using WebLibServer.Internal;

namespace WebLibServer.Videos;

public class VideoAnalyzer
{
	private readonly ILogger _logger = Defaults.LoggerFactory.CreateLogger<VideoAnalyzer>();

	public VideoAnalyzer(string vidFilePath)
	{
		_logger.LogInformation("Analyzing video file with FFProbe at {Path}", vidFilePath);
		Analysis = FFProbe.Analyse(vidFilePath);

		if (Analysis.PrimaryVideoStream == null)
			throw new Exception("Video stream not found.");
	}

	public IMediaAnalysis Analysis { get; private set; }

	public VideoStream Vs => Analysis.PrimaryVideoStream;

	public AudioStream As => Analysis.PrimaryAudioStream;

	public VideoResolution GetResolution() =>
		VideoResolution.GetByDimension(Vs.Width, Vs.Height);

	public TimeSpan Duration => Analysis.Duration;
}
