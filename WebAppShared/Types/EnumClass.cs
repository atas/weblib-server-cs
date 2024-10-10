namespace WebAppShared.Types;

public class EnumClass<T>
{
    protected readonly T Value;
    public EnumClass(T value)
    {
        Value = value;
    }
    
    public EnumClass()
    {
    }
    
    public static implicit operator string(EnumClass<T> e)
    {
        return e.ToString();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(EnumClass<T> a, EnumClass<T> b)
    {
        return a?.ToString() == b?.ToString();
    }

    public static bool operator !=(EnumClass<T> a, EnumClass<T> b)
    {
        return !(a == b);
    }
    
    protected bool Equals(EnumClass<T> other)
    {
        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EnumClass<T>)obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}