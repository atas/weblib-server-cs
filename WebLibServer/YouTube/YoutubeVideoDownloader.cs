/*
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace WebLibServer.YouTube;

public class YoutubeVideoDownloader(ILogger<YoutubeVideoDownloader> logger)
{
	public async Task<string> Download(string youtubeUrl)
	{
		var youtube = new YoutubeClient();

		// var streamManifest = await youtube.Videos.Streams.GetManifestAsync(_youtubeUrl);

		// Get highest quality muxed stream
		// var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

		// Get the actual stream
		// var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

		/#1#/ ...or highest bitrate audio-only stream
		var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

		// ...or highest quality MP4 video-only stream
		var mp4VideoStream = streamManifest
			.GetVideoOnlyStreams()
			.Where(s => s.Container == Container.Mp4)
			.GetWithHighestVideoQuality();#1#


		var streamManifest = await youtube.Videos.Streams.GetManifestAsync(youtubeUrl);
		var streamInfos = streamManifest.GetMuxedStreams().ToList();

		IVideoStreamInfo streamInfo = streamInfos.Where(s => s.VideoQuality.MaxHeight <= 1080)
			.MaxBy(s => s.VideoQuality.MaxHeight);
		streamInfo ??= streamInfos.GetWithHighestVideoQuality();

		var outputFilePath = Path.Join(Path.GetTempPath(), Guid.NewGuid() + ".mp4");
		logger.LogInformation("Downloading youtube video {Youtube} {Width}x{Height} to {Path}", youtubeUrl,
			streamInfo.VideoResolution.Width, streamInfo.VideoResolution.Height, outputFilePath);

		await youtube.Videos.Streams.DownloadAsync(streamInfo, outputFilePath);

		return outputFilePath;
	}
}
*/
