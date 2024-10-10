using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Net.Http.Headers;
using MoreLinq.Extensions;

namespace WebAppShared.Http;

/// <summary>
/// Renders a given Razor view to a string.
/// </summary>
public static class ConfigureHttpClient
{

	private static readonly Dictionary<string, string> ChromeHeaders = new()
	{
		[HeaderNames.UserAgent] =
			"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.0.0 Safari/537.36",
		[HeaderNames.Accept] =
			"text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
		[HeaderNames.AcceptEncoding] = "gzip, deflate, br",
		[HeaderNames.AcceptLanguage] = "en-GB,en;q=0.9",
		[HeaderNames.CacheControl] = "no-cache",
		[HeaderNames.Pragma] = "no-cache",
		["sec-ch-ua"] = "\"Chromium\";v=\"104\", \" Not A;Brand\";v=\"99\", \"Google Chrome\";v=\"104\"",
		["sec-ch-ua-mobile"] = "?0",
		["sec-ch-ua-platform"] = "\"Windows\"",
		["sec-fetch-dest"] = "document",
		["sec-fetch-mode"] = "navigate",
		["sec-fetch-site"] = "none",
		["sec-fetch-user"] = "?1",
		["dnt"] = "1",
		["Upgrade-Insecure-Requests"] = "1",
	};

	public static HttpClientHandler ChromeClientHandler()
	{
		var httpClientHandler = new HttpClientHandler
		{
			AllowAutoRedirect = true,
			MaxAutomaticRedirections = 5,
			MaxConnectionsPerServer = 1,
			UseCookies = true,
			CookieContainer = new CookieContainer(),
		};

		if (httpClientHandler.SupportsAutomaticDecompression)
			httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
		else
			ChromeHeaders[HeaderNames.AcceptEncoding] = "";

		return httpClientHandler;
	}

	/// <summary>
	/// Configures an HttpClient to be like Google Chrome.
	/// </summary>
	/// <param name="client"></param>
	public static void ChromeClient(HttpClient client)
	{
		client.Timeout = TimeSpan.FromSeconds(10);
		ChromeHeaders.ForEach(h => client.DefaultRequestHeaders.Add(h.Key, h.Value));
	}

	/// <summary>
	/// Gets the named HttpClient that behaves like Google Chrome
	/// </summary>
	/// <param name="httpClientFactory"></param>
	/// <returns></returns>
	public static HttpClient ChromeClient(this IHttpClientFactory httpClientFactory)
	{
		return httpClientFactory.CreateClient("chrome");
	}
}
