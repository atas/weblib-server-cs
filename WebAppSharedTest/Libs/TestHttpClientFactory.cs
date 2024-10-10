using WebAppShared.Http;

namespace WebAppSharedTest.Libs;

public class TestHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name)
    {
        var client = new HttpClient();
        ConfigureHttpClient.ChromeClient(client);
        return client;
    }
}