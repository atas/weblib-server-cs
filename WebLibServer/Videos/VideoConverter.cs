using System;
using System.IO;
using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.Extensions.Logging;
using WebLibServer.Internal;

namespace WebLibServer.Videos;

public class VideoConverter(string inputFile, VideoAnalyzer va)
{
	private readonly ILogger<VideoConverter> _logger = Defaults.LoggerFactory.CreateLogger<VideoConverter>();
	private readonly string _convertedVidFilePath = Path.Join(Path.GetTempPath(), Guid.NewGuid() + ".mp4");

	private readonly VideoSize _videoSize = VideoSize.Original;
	private AudioQuality? _audioQuality;

	public string Run()
	{
		try
		{
			_logger.LogInformation("CONVERTING THE VIDEO WITH FFMPEG");
			FFMpegArguments
				.FromFileInput(inputFile)
				.OutputToFile(_convertedVidFilePath, true, Args)
				.ProcessSynchronously();
		}
		catch (Exception)
		{
			if (File.Exists(_convertedVidFilePath)) File.Delete(_convertedVidFilePath);
			throw;
		}

		return _convertedVidFilePath;
	}

	private void Args(FFMpegArgumentOptions opts)
	{
		if (va.As.BitRate > 128000) _audioQuality = AudioQuality.Normal;

		opts.WithVideoCodec(VideoCodec.LibX264).WithConstantRateFactor(21);
		opts.WithAudioCodec(AudioCodec.Aac);

		if (_audioQuality != null) opts.WithAudioBitrate(_audioQuality.Value);

		if (_videoSize != VideoSize.Original)
			opts.WithVideoFilters(filterOptions => filterOptions.Scale(_videoSize));

		opts.WithFastStart();
		// .WithVariableBitrate(4)
	}
}
