using Microsoft.AspNetCore.Routing;

namespace WebLibServer.WebSys.MvcRouting;

public class ApiParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object value)
    {
        if (value == null) return null;

        var val = value.ToString();

        if (val != null && val.EndsWith("api", StringComparison.InvariantCulture)) return val.Substring(0, val.Length - 3);

        return val;
    }
}