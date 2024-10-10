using JetBrains.Annotations;

namespace WebLibServer.Emailing;

[UsedImplicitly]
public class EmailerConfigBase
{
    public int SmtpPort { get; set; }
    public bool SmtpEncryption { get; set; }

    public string Hostname { get; set; }
    public string SiteName { get; set; }
    public string SiteEmail { get; set; }
}
