using JetBrains.Annotations;

namespace WebAppShared.Config;

[UsedImplicitly]
public class InfluxConfig
{
    public string Url { get; set; }
    public string Token { get; set; }
    public string Bucket { get; set; }
    public string Org { get; set; }
}
