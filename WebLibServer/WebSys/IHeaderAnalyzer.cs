namespace WebLibServer.WebSys;

public interface IHeaderAnalyzer
{
    string CloudFlareIpAddress();
    string XForwardedForIpAddress();
    string CloudFlareIpAddressCountry();
}