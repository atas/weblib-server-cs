using JetBrains.Annotations;

namespace WebAppShared.RequestModels;

public class ScraperApiUserSyncReqModel
{
	public string Username { get; set; }

	public string ProfileName { get; set; }
	public string ScrapeRefUrl { get; set; }
	public string PhotoUrl { get; set; }

	[CanBeNull]
	public string Bio { get; set; }
}
