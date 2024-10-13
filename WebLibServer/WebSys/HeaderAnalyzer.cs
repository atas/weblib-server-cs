using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebLibServer.WebSys;

/// <summary>
///     Analyzes the headers and returns relevant information
/// </summary>
[UsedImplicitly]
public class HeaderAnalyzer(IHttpContextAccessor httpContext, ILogger<HeaderAnalyzer> logger) : IHeaderAnalyzer
{
    private readonly IHeaderDictionary _headers = httpContext.HttpContext?.Request?.Headers;
    private readonly ILogger _logger = logger;

    private string _cloudFlareIpAddress;

    public string CloudFlareIpAddress()
    {
        if (_cloudFlareIpAddress != null) return _cloudFlareIpAddress;

        if (_headers.ContainsKey(IpAddressHeaders.CloudFlareConnectingIp))
        {
            _cloudFlareIpAddress = _headers[IpAddressHeaders.CloudFlareConnectingIp].ToString();
            _logger.LogDebug("Obtained user's IP Address from CF-CONNECTING-IP header as {0}", _cloudFlareIpAddress);
            return _cloudFlareIpAddress;
        }

        return null;
    }

    public string XForwardedForIpAddress()
    {
        if (_headers.ContainsKey(IpAddressHeaders.XForwarded))
        {
            var ipAddress = _headers[IpAddressHeaders.XForwarded].ToString();

            _logger.LogDebug("Obtained user's IP Address from HTTP_X_FORWARDED_FOR header as {0}", ipAddress);

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0) return addresses[0];
            }
        }

        return null;
    }

    public string CloudFlareIpAddressCountry()
    {
        if (!_headers.ContainsKey(IpAddressHeaders.CloudFlareIpCountry))
            return "IE";
        return _headers[IpAddressHeaders.CloudFlareIpCountry].ToString();
    }


    private static class IpAddressHeaders
    {
        public const string CloudFlareConnectingIp = "CF-Connecting-IP";
        public const string XForwarded = "X-Forwarded-For";

        public const string CloudFlareIpCountry = "cf-ipcountry";
    }
}