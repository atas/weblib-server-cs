using WebLibServer.SharedLogic.Fx;
using WebLibServer.WebSys.DI;

namespace WebLibServer.SharedLogic.FxConverter;

[Service]
public class FxConverter(FxRateSvc fxRateSvc)
{
    public decimal Convert(decimal balance, Currency fromCurrency, Currency toCurrency)
    {
        if (fromCurrency == toCurrency) return balance;
        if (fromCurrency == null) throw new Exception("fromCurrency is null");
        if (toCurrency == null) throw new Exception("toCurrency is null");

        var rateByEuro = fxRateSvc.GetRate(fromCurrency);

        var euroFromValue = balance / rateByEuro;

        var rateFromEuro = fxRateSvc.GetRate(toCurrency);

        return Math.Round(euroFromValue * rateFromEuro, 2);
    }
}