using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace WebAppShared.WebSys.MvcRouting;

public class SlugifyParameterTransformer(ILogger<SlugifyParameterTransformer> logger) : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        // Slugify value
        var val = value == null
            ? null
            : Regex.Replace(value.ToString() ?? string.Empty, "([a-z])([A-Z])", "$1-$2", RegexOptions.None,
                TimeSpan.FromMilliseconds(100)).ToLower();

        logger.LogInformation("SlugifyParameterTransformer: {Value} -> {Slug}", value, val ?? "null");

        return val;
    }
}