using System.Globalization;
using System.Xml.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebAppShared.WebSys.DI;

namespace WebAppShared.SharedLogic.FxConverter;

/// <summary>
///     Fetches Daily Fx Rates from ECB
/// </summary>
[Service, UsedImplicitly]
public class FxRateFetcher(ILogger<FxRateFetcher> logger)
{
    public (DateTime fromDate, Dictionary<string, decimal> rates) Fetch()
    {
        logger.LogInformation("Fetching ECB Daily FX Rates from europa.eu");

        var doc = XDocument.Load("https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");

        var dateStr = doc.Root?.Elements().Where(n => n.Name.LocalName == "Cube")
            .Elements().Single().Attribute("time")?.Value;

        if (dateStr == null) throw new NullReferenceException("Eurofxref date is null");

        var fxFromDate = DateTime.ParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var rates = doc.Root?.Elements().Where(n => n.Name.LocalName == "Cube").Elements().Elements()
            .ToDictionary(n => n.Attribute("currency")?.Value, n => decimal.Parse(n.Attribute("rate")?.Value ?? "0"));

        // EUR is considered 1 from eurofxref
        rates?.TryAdd("EUR", 1.00m);

        logger.LogInformation("Fetched ECB Daily FX Rates from europa.eu, rates are from {Date} there are {No} rates",
            fxFromDate.ToString("yyyy-MM-dd"), rates.Count);

        return (fxFromDate, rates);
    }
}