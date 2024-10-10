using JetBrains.Annotations;

namespace WebLibServer.Uploaders.R2;

[UsedImplicitly]
public class R2FileUploaderConfig
{
	public string Key { get; set; }
	public string Secret { get; set; }

	public string ServiceUrl { get; set; }
}
