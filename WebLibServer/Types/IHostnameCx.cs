namespace WebLibServer.Types;

public interface IHostnameCx
{
    string SiteHostname { get; }
    string SiteHostnameWithoutPort { get; }
    string RequestedHostname { get; }
}