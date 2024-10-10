using System.Text;
using JetBrains.Annotations;
using WebLibServer.Utils;

namespace WebLibServer.SharedLogic.Sitemaps;

[UsedImplicitly]
public class SitemapItem
{
    public string Loc { get; set; }

    public DateTime? LastMod { get; set; }

    public SitemapItemChangeFreq? ChangeFreq { get; set; }

    public decimal? Priority { get; set; }

    public SitemapVideoItem VideoItem { get; set; }

    public override string ToString()
    {
        if (Loc == null) throw new Exception("SitemapItem.Loc is null.");

        var sb = new StringBuilder();
        sb.Append("<url>\n");
        sb.Append($"<loc>{Loc}</loc>\n");

        if (LastMod != null)
            sb.Append($"<lastmod>{LastMod.Value.ToString("yyyy-MM-dd")}</lastmod>\n");

        if (ChangeFreq != null)
            sb.Append($"<changefreq>{ChangeFreq.GetDescAttr()}</changefreq>\n");

        if (Priority != null)
            sb.Append($"<priority>{Priority.Value.ToString("F1")}</priority>\n");

        if (VideoItem != null) sb.Append(VideoItem);

        sb.Append("\n</url>");

        return sb.ToString();
    }
}