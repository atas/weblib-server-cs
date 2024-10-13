using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WebLibServer.Images;
using WebLibServer.Internal;
using Size = System.Drawing.Size;

namespace WebLibServer.Videos;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class VideoSpriteGenerator : IDisposable
{
    private readonly string _videoPath;
    private readonly VideoAnalyzer _videoAnalyzer;

    private readonly ILogger<VideoSpriteGenerator>
        _logger = Defaults.LoggerFactory.CreateLogger<VideoSpriteGenerator>();

    private Image<Rgba32> _sprite;

    /// <summary>
    /// A sprite image's width and height
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public int WidthHeight = 150;

    /// <summary>
    /// Minimum sprite image snapshot interval in seconds
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public int MinInterval = 5;

    /// <summary>
    /// Maximum number of sprite image to generate per video
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public int MaxCount = 50;

    private readonly object _imageMergeLock = new();

    public VideoSpriteGenerator(string videoPath, VideoAnalyzer videoAnalyzer)
    {
        _videoPath = videoPath;
        _videoAnalyzer = videoAnalyzer;
    }

    /// <summary>
    /// Generate and return a sprite image in memory
    /// </summary>
    /// <returns></returns>
    public async Task<List<SpriteResult>> Generate()
    {
        _logger.LogInformation("Running Sprite Generator on file {File}", _videoPath);

        var imageCount = CalculateImageCount();
        var interval = _videoAnalyzer.Duration.TotalSeconds / imageCount;

        var thumbnailSize = new MaxImageSizeByRatio(_videoAnalyzer.Vs.Width, _videoAnalyzer.Vs.Height, WidthHeight);

        _logger.LogInformation("Generating {Count} sprite thumbnails with interval {Interval}s and {Width}x{Height}px",
            imageCount, interval, thumbnailSize.Width, thumbnailSize.Height);

        _sprite = new Image<Rgba32>(thumbnailSize.Width * imageCount, thumbnailSize.Height);

        var tasks = new Task[imageCount];

        for (var i = 0; i < imageCount; i++)
        {
            var currentIndex = i;
            var time = TimeSpan.FromSeconds(currentIndex * interval);
            var size = new Size(thumbnailSize.Width, thumbnailSize.Height);
            var isSucceeded = await FFMpeg.SnapshotAsync(_videoPath, _videoPath + ".jpg", size, time);

            if (!isSucceeded) throw new Exception("Failed to generate snapshot.");

            using Image img = await Image.LoadAsync(_videoPath + ".jpg");
            MergeAndSaveImages(img, currentIndex);
        }

        // await Parallel.ForEachAsync(tasks,
        //     new ParallelOptions
        //         { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling(Environment.ProcessorCount / 2.0)) },
        //     async (t, ct) => await t);

        _logger.LogInformation("Video Sprite Generator Finished for file {File}", _videoPath);

        return new List<SpriteResult>
        {
            new()
            {
                Guid = Guid.NewGuid(),
                Image = _sprite,
                StartTime = 0,
                Interval = interval,
                Width = thumbnailSize.Width,
                Height = thumbnailSize.Height,
                Count = imageCount
            }
        };
    }

    public int CalculateImageCount() => Math.Min(MaxCount, (int)(_videoAnalyzer.Duration.TotalSeconds / MinInterval));

    private void MergeAndSaveImages(Image thumbnail, int i)
    {
        if (thumbnail == null) return;

        lock (_imageMergeLock)
        {
            var drawPoint = new Point(i * thumbnail.Width, 0);
            // _logger.LogInformation(
            // 	"Going to draw the thumbnail {ThumbW}x{ThumbH} onto sprite {SpriteW}x{SpriteH} at point {PointX}x{PointY}",
            // 	image.Width, image.Height, _sprite.Width, _sprite.Height, drawPoint.X, drawPoint.Y);

            // take the 2 source images and draw them onto the image
            _sprite.Mutate<Rgba32>(o => { o.DrawImage(thumbnail, drawPoint, new GraphicsOptions()); });
        }
    }

    public class SpriteResult
    {
        public Guid Guid { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonIgnore]
        public Image<Rgba32> Image { get; set; }

        public double StartTime { get; set; }

        /// <summary>
        /// The interval of sprite images in seconds as double value
        /// </summary>
        public double Interval { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int Count { get; set; }
    }

    public void Dispose()
    {
        _sprite?.Dispose();
    }
}