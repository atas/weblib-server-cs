namespace WebLibServer.Contexts;

public interface IConnectionCx
{
	/// <summary>
	/// Iso 2 character code of the connecting IP Address country. Defaults to "IE" if doesn't exist.
	/// </summary>
	string IpAddressCountry { get; }

	string IpAddress { get; }
	string BrowserAgent { get; }
}
