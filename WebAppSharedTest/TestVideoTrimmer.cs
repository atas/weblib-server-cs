using WebAppShared.Videos;
using WebAppSharedTest.TestLibs;
using Xunit.Abstractions;

namespace WebAppSharedTest;

public class TestVideoTrimmer(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public async Task TestSeek()
    {
        var videoPath = Environment.CurrentDirectory + "/../../../_Data/SampleVideo-1m40s.mp4";
        try
        {
            var trimmer = new VideoTrimmer();
            await trimmer.Seek(videoPath, videoPath + "-generated.mp4", TimeSpan.FromSeconds(3.11));

            Assert.True(File.Exists(videoPath + "-generated.mp4"));

            var va = new VideoAnalyzer(videoPath + "-generated.mp4");
            Assert.Equal(98, (int)va.Duration.TotalSeconds);
        }
        finally
        {
            if (File.Exists(videoPath + "-generated.mp4"))
                File.Delete(videoPath + "-generated.mp4");
        }
    }
}