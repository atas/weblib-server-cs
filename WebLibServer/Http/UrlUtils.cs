using System;
using System.Text.RegularExpressions;

namespace WebLibServer.Http;

public static class UrlUtils
{
	/// <summary>
	/// Removes http:// and www. from the URL
	/// </summary>
	public static string GetUrlWithBaseDomain(string url)
	{
		var uri = new Uri(url);
		var urlWithBaseDomain = (uri.Host + uri.PathAndQuery).ToLower();

		if (urlWithBaseDomain.StartsWith("www.")) urlWithBaseDomain = urlWithBaseDomain.Substring(4);

		return urlWithBaseDomain;
	}

	public static string ConvertToSlug(string urlPart)
	{
		if (urlPart == null) return null;

		urlPart = urlPart.Trim().ToLowerInvariant();


		var replacing = Regex.Replace(urlPart, "[^a-z0-9- ]+", "");
		replacing = Regex.Replace(replacing.Trim(), @"\s+", "-");
		replacing = Regex.Replace(replacing, "-+", "-");

		return replacing.Trim('-');
	}
}
