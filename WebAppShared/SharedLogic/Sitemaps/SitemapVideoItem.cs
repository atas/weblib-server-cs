using System.Text;
using JetBrains.Annotations;

namespace WebAppShared.SharedLogic.Sitemaps;

[UsedImplicitly]
public class SitemapVideoItem
{
    // <video:title>
    public string Title { get; set; }

    // <video:thumbnail_loc>
    public string ThumbnailLoc { get; set; }

    // <video:content_loc>
    public string ContentLoc { get; set; }

    // <video:player_loc>
    public string PlayerLoc { get; set; }

    // <video:duration>
    public int Duration { get; set; }

    // <video:view_count>
    public int ViewCount { get; set; }

    // <video:family_friendly>yes</video:family_friendly>
    public bool FamilyFriendly { get; set; }

    // <video:uploader info="https://URL">username</video:uploader>
    /// <summary>
    ///     Item1: Username, Item2: URL
    /// </summary>
    public Tuple<string, string> Uploader { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        AddToSb(sb, "title", Title);
        AddToSb(sb, "thumbnail_loc", ThumbnailLoc);
        AddToSb(sb, "content_loc", ContentLoc);
        AddToSb(sb, "player_loc", PlayerLoc);
        AddToSb(sb, "duration", Duration);
        AddToSb(sb, "view_count", ViewCount);
        AddToSb(sb, "family_friendly", FamilyFriendly ? "yes" : "no");

        if (Uploader != null)
            sb.Append($"<video:uploader info=\"{Uploader.Item2}\">{Uploader.Item1}</video:uploader>\n");

        if (sb.Length == 0) return "";

        return $"<video:video>\n{sb}\n</video:video>\n";
    }

    private static void AddToSb(StringBuilder sb, string tag, object content)
    {
        if (string.IsNullOrWhiteSpace(content.ToString())) return;
        sb.Append($"<video:{tag}>{content}</video:{tag}>\n");
    }
}