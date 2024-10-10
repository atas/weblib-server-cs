using WebAppShared.YouTube;

namespace WebAppSharedTest;

public class YoutubeVideoIdParserTest
{
	[Fact]
	public void ParseYoutubeVideoId()
	{
		var parser = new YoutubeVideoIdParser();

		Assert.Equal("-wtIMTCHWuI", parser.Parse("https://www.youtube.com/watch?v=-wtIMTCHWuI"));
		Assert.Equal("-wtIMTCHWuI", parser.Parse("https://www.youtube.com/watch?v=-wtIMTCHWuI&test=test"));
		Assert.Equal("-wtIMTCHWuI", parser.Parse("https://www.youtube.com/watch?test=dsf&v=-wtIMTCHWuI"));

		Assert.Equal("-wtIMTCHWuI", parser.Parse("https://www.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1"));

		Assert.Equal("-wtIMTCHWuI", parser.Parse("https://youtu.be/-wtIMTCHWuI"));

		Assert.Equal("DYQLoEdEy6c", parser.Parse("https://www.youtube.com/shorts/DYQLoEdEy6c"));
	}
}
