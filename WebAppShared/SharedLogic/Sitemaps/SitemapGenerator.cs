using System.IO.Compression;
using System.Text;
using JetBrains.Annotations;
using LinqKit;
using Microsoft.AspNetCore.Hosting;
using WebAppShared.Exceptions;
using WebAppShared.Types;
using WebAppShared.WebSys;
using WebAppShared.WebSys.DI;

namespace WebAppShared.SharedLogic.Sitemaps;

[Service, UsedImplicitly]
public class SitemapGenerator(
    IWebHostEnvironment webHostEnvironment,
    IHostnameCx hostnameCx)
    : IDisposable
{
    private int _currentSitemapId;
    private int _currentSitemapItemsCount;

    private GZipStream _gs;
    private long _writtenBytes;
    public string BaseDirectory { get; set; } = Path.Join(webHostEnvironment.WebRootPath, "public", "sitemaps");

    public void Dispose()
    {
        _gs?.Dispose();
    }

    public async Task AddItem(SitemapItem item)
    {
        _currentSitemapItemsCount++;
        var data = Encoding.UTF8.GetBytes(item.ToString());
        await (await GetSitemapStream()).WriteAsync(data);
        _writtenBytes += data.Length;
    }
    
    public void ClearOldSitemaps()
    {
        var oldFiles = Directory.GetFiles(BaseDirectory, "sitemap-*.xml");
        oldFiles.ForEach(File.Delete);

        oldFiles = Directory.GetFiles(BaseDirectory, "sitemap-*.xml.gz");
        oldFiles.ForEach(File.Delete);
    }

    public async Task GenerateSitemapIndex()
    {
        await using var fs =
            new FileStream(
                Path.Join(BaseDirectory, "index.xml"),
                FileMode.Create);

        var header =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<sitemapindex xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n"u8.ToArray();
        await fs.WriteAsync(header);

        for (var i = 1; i <= _currentSitemapId; i++)
        {
            var sitemap =
                $"<sitemap><loc>https://{hostnameCx.SiteHostname}/public/sitemaps/sitemap-{i}.xml.gz</loc></sitemap>\n";
            await fs.WriteAsync(Encoding.UTF8.GetBytes(sitemap));
        }

        await fs.WriteAsync("</sitemapindex>"u8.ToArray());

        await fs.FlushAsync();
    }
    
    private async Task<Stream> GetSitemapStream()
    {
        if (_currentSitemapItemsCount > 49900 || _gs == null || _writtenBytes > 48000000)
        {
            await WriteSitemapFooter();
            _gs?.Dispose();
            _currentSitemapItemsCount = 0;
            _writtenBytes = 0;

            _currentSitemapId++;

            var fs = new FileStream(GetSitemapFilePath(), FileMode.Create);
            _gs = new GZipStream(fs, CompressionLevel.SmallestSize, false);

            await WriteSitemapHeader();
        }

        return _gs;
    }

    private string GetSitemapFilePath()
    {
        return Path.Join(BaseDirectory, $"sitemap-{_currentSitemapId.ToString()}.xml.gz");
    }

    private async Task WriteSitemapHeader()
    {
        if (_gs == null) return;

        var sitemapHeader =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">\n";

        await _gs.WriteAsync(Encoding.UTF8.GetBytes(sitemapHeader));
    }

    public async Task WriteSitemapFooter()
    {
        if (_gs == null) return;
        await _gs.WriteAsync("\n</urlset>"u8.ToArray());
    }
}