namespace WebAppShared.WebSys;

public interface IHeaderAnalyzer
{
    string CloudFlareIpAddress();
    string XForwardedForIpAddress();
    string CloudFlareIpAddressCountry();
}