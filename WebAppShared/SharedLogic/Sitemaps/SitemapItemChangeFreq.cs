using System.ComponentModel;
using JetBrains.Annotations;

namespace WebAppShared.SharedLogic.Sitemaps;

[UsedImplicitly]
public enum SitemapItemChangeFreq
{
    [Description("always")] Always,

    [Description("hourly")] Hourly,

    [Description("daily")] Daily,

    [Description("weekly")] Weekly,

    [Description("monthly")] Monthly,

    [Description("yearly")] Yearly,

    [Description("never")] Never
}