using JetBrains.Annotations;
using WebLibServer.Contexts;
using WebLibServer.WebSys.DI;

namespace WebLibServer.SharedLogic.Fx;

[Service, UsedImplicitly]
public class DefaultLocationCurrencyCx(IConnectionCx connectionCx)
{
    public Fx.Currency Get()
    {
        var ct = connectionCx.IpAddressCountry;

        if (ct is "GB" or "UK")
            return Fx.Currency.GBP;

        if (ct is "US" or "CA")
            return Fx.Currency.USD;

        return Fx.Currency.EUR;
    }

    public override string ToString()
    {
        return Get();
    }

    public static implicit operator Fx.Currency(DefaultLocationCurrencyCx c)
    {
        return c.Get();
    }
}