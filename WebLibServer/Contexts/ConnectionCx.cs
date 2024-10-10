using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebLibServer.WebSys;

namespace WebLibServer.Contexts;
[UsedImplicitly]
public class ConnectionCx(
    IHttpContextAccessor httpContext,
    ILogger<ConnectionCx> logger,
    IHeaderAnalyzer headerAnalyzer) : IConnectionCx
{
    private string _ipAddress;

    /// <summary>
    ///     Iso 2 character code of the connecting IP Address country. Defaults to "IE" if doesn't exist.
    /// </summary>
    public string IpAddressCountry => headerAnalyzer.CloudFlareIpAddressCountry();

    public string IpAddress => _ipAddress ?? _GetIpAddress();

    public string BrowserAgent => httpContext.HttpContext?.Request.Headers.UserAgent;

    private string _GetIpAddress()
    {
        // If CloudFlare IP Address is found, use that
        if (headerAnalyzer.CloudFlareIpAddress() != null)
            return _ipAddress = headerAnalyzer.CloudFlareIpAddress();

        // If XForwardedFor IP Address is found, use that
        if (headerAnalyzer.XForwardedForIpAddress() != null)
            return _ipAddress = headerAnalyzer.XForwardedForIpAddress();

        // If no headers contain any IP Address, use the connect IP Address
        // This shouldn't happen in production, we are always behind Nginx and CloudFlare.
        _ipAddress = httpContext.HttpContext?.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        logger.LogDebug("Obtaining user's IP Address from connection IPv4 {IpAddress}", _ipAddress);

        return _ipAddress;
    }
}