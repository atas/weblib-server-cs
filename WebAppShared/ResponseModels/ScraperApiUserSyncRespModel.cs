using System.Collections.Generic;

namespace WebAppShared.ResponseModels;

public class ScraperApiUserSyncRespModel
{
	public int UserId { get; set; }
	public List<string> PostRefUrls { get; set; }
}
