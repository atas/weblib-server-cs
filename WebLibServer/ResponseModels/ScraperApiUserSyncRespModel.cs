using System.Collections.Generic;

namespace WebLibServer.ResponseModels;

public class ScraperApiUserSyncRespModel
{
	public int UserId { get; set; }
	public List<string> PostRefUrls { get; set; }
}
