using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebAppShared.Http;
using WebAppShared.Internal;

namespace WebAppShared.Videos;

/// <summary>
/// Provides a local path to a video file, either directly or by downloading from its cloud storage.
/// </summary>
public class LocalVideoPath : IDisposable
{
	private readonly IHttpClientFactory _httpClientFactory;

	private readonly ILogger<LocalVideoPath> _logger =
		Defaults.LoggerFactory.CreateLogger<LocalVideoPath>();

	private string _localVidPath;
	private readonly string _httpUrl;

	private bool _disposeLocalVidFile;

	public LocalVideoPath(IHttpClientFactory httpClientFactory, string fromUrl)
	{
		_httpClientFactory = httpClientFactory;
		_httpUrl = fromUrl;
	}

	public LocalVideoPath(string localVidPath)
	{
		_localVidPath = localVidPath;
	}

	public async Task<string> GetLocalPath()
	{
		if (_localVidPath == null && _httpUrl != null) await Download();

		return _localVidPath;
	}

	private async Task Download()
	{
		var httpStream = await _httpClientFactory.ChromeClient().GetStreamAsync(_httpUrl);

		var tempVideoPath = Path.GetTempFileName();
		await using Stream file = File.Create(tempVideoPath);
		await httpStream.CopyToAsync(file);
		_localVidPath = tempVideoPath;
		_disposeLocalVidFile = true;

		_logger.LogInformation("Downloading video {VideoUrl} to generate its preview", _httpUrl);
	}

	public void Dispose()
	{
		if (_disposeLocalVidFile && File.Exists(_localVidPath))
		{
			_logger.LogInformation("Disposing of {Path}", _disposeLocalVidFile);
			File.Delete(_localVidPath);
		}
	}
}
