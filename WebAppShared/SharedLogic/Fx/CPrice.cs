namespace WebAppShared.SharedLogic.Fx;

public class CPrice
{
    public CPrice()
    {
    }

    public CPrice(Fx.Currency currency, decimal? price)
    {
        Currency = currency;
        Price = price;
    }

    public Fx.Currency Currency { get; set; }
    public decimal? Price { get; set; }

    public static CPrice operator *(CPrice a, int i)
    {
        return new CPrice(a.Currency, a.Price * i);
    }

    public static CPrice operator *(int i, CPrice a)
    {
        return new CPrice(a.Currency, a.Price * i);
    }
}