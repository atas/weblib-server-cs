using Newtonsoft.Json;
using WebLibServer.Types;
using WebLibServer.WebSys.Json;

namespace WebLibServer.SharedLogic.Fx;

[JsonConverter(typeof(ToStringJsonConverter))]
public class Currency : EnumClass<string>, IEnumLike<string>
{
    private static readonly HashSet<string> AvailableValues = ["EUR", "USD", "GBP"];

    public static readonly Currency EUR = new("EUR");
    public static readonly Currency USD = new("USD");
    public static readonly Currency GBP = new("GBP");

    private readonly string _value;

    public Currency()
    {

    }

    public Currency(string val) : base(val)
    {
        if (!AvailableValues.Contains(val) && val != null)
            throw new Exception("The currency is not available.");

        _value = val;
    }

    public static List<string> GetValues()
    {
        return AvailableValues.ToList();
    }

    public override string ToString()
    {
        return _value;
    }

    public static implicit operator string(Currency c)
    {
        return c?.ToString();
    }

    public static implicit operator Currency(string c)
    {
        return new Currency(c);
    }

    public static bool operator ==(Currency a, Currency b)
    {
        return a?.ToString() == b?.ToString();
    }

    public static bool operator !=(Currency a, Currency b)
    {
        return !(a == b);
    }

    protected bool Equals(Currency other)
    {
        return _value == other._value;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Currency)obj);
    }

    public override int GetHashCode()
    {
        return _value != null ? _value.GetHashCode() : 0;
    }
}