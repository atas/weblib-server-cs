using System;
using System.Text.RegularExpressions;
using System.Web;
using WebAppShared.Exceptions;

namespace WebAppShared.YouTube;

public class YoutubeVideoIdParser
{
	private static readonly Regex WatchLinkPattern = new("^https?://(www.)?youtube.com/watch");

	private static readonly Regex VLinkPattern = new("^https?://(www.)?youtube.com/v/([^?]+)");

	private static readonly Regex BeLinkPattern = new("^https?://(www.)?youtu.be/([^?]+)");

	private static readonly Regex ShortLinkPattern = new("^https?://(www.)?youtube.com/shorts/([^?/]+)");

	/// <summary>
	/// Possible Youtube URLs
	/// http://www.youtube.com/watch?v=-wtIMTCHWuI
	/// http://www.youtube.com/v/-wtIMTCHWuI?version=3&autohide=1
	/// http://youtu.be/-wtIMTCHWuI
	/// </summary>
	/// <param name="youtubeUrl"></param>
	/// <returns></returns>
	public string Parse(string youtubeUrl)
	{
		string videoId = null;

		if (WatchLinkPattern.IsMatch(youtubeUrl))
			videoId = GetFromWatchLink(youtubeUrl);

		else if (VLinkPattern.IsMatch(youtubeUrl))
			videoId = GetFromVLink(youtubeUrl);

		else if (BeLinkPattern.IsMatch(youtubeUrl))
			videoId = GetFromBeLink(youtubeUrl);

		else if (ShortLinkPattern.IsMatch(youtubeUrl))
			videoId = GetFromShortLink(youtubeUrl);

		else
			throw new HttpJsonError("Cannot recognise the YouTube URL.");

		if (string.IsNullOrEmpty(videoId))
			throw new HttpJsonError("YouTube url does not contain a v= parameter.");

		return videoId;
	}

	private string GetFromWatchLink(string youtubeUrl)
	{
		var uri = new Uri(youtubeUrl);
		var qs = HttpUtility.ParseQueryString(uri.Query);

		return qs["v"];
	}

	private string GetFromVLink(string youtubeUrl)
	{
		var matches = VLinkPattern.Match(youtubeUrl);
		return matches.Groups[2].Value;
	}

	private string GetFromBeLink(string youtubeUrl)
	{
		var matches = BeLinkPattern.Match(youtubeUrl);
		return matches.Groups[2].Value;
	}

	private string GetFromShortLink(string youtubeUrl)
	{
		var matches = ShortLinkPattern.Match(youtubeUrl);
		return matches.Groups[2].Value;
	}
}
