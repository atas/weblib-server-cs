using System;
using System.Globalization;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.Extensions.Logging;
using WebAppShared.Internal;

namespace WebAppShared.Videos;

public class VideoTrimmer
{
	private readonly ILogger<VideoTrimmer> _logger = Defaults.LoggerFactory.CreateLogger<VideoTrimmer>();

	public async Task Seek(string source, string target, TimeSpan fromTime)
	{
		_logger.LogInformation("Trimming video at {Source} to start from {Time} second", source, fromTime);

		var ffmpeg = new Ffmpeg();
		ffmpeg.Arg("i", source)
			.Arg("c", "copy")
			.Arg("ss", fromTime.ToString(@"hh\:mm\:ss"))
			.Dest(target);

		await ffmpeg.ExecAsync();

		_logger.LogInformation("Trimmed video and saved to {Target}", target);
	}
}
