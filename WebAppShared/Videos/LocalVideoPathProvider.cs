using JetBrains.Annotations;

namespace WebAppShared.Videos;

[UsedImplicitly]
public class LocalVideoPathProvider(IHttpClientFactory httpClientFactory)
{
	public LocalVideoPath FromLocalPath(string localVidPath) => new(localVidPath);

	public LocalVideoPath FromUrl(string url) => new(httpClientFactory, url);
}
