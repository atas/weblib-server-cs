using System.Collections.Generic;

namespace WebAppShared.Types;

/// <summary>
/// This interface is used to create enum-like classes.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEnumLike<T>
{
    static abstract List<T> GetValues();
}
