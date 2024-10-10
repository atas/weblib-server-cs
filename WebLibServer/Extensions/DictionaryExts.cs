using JetBrains.Annotations;

namespace WebLibServer.Extensions;

public static class DictionaryExts
{
    /// <summary>
    /// Merges the source dictionary into the target dictionary.
    /// </summary>
    /// <param name="target">The dictionary to merge into.</param>
    /// <param name="source">The dictionary to merge from.</param>
    /// <param name="overwrite">Whether to overwrite existing entries in the target dictionary.</param>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> target, [CanBeNull] Dictionary<TKey, TValue> source, bool overwrite = true)
    {
        var result = new Dictionary<TKey, TValue>(target);

        if (source == null) return result;

        foreach (var item in source)
        {
            if (overwrite || !result.ContainsKey(item.Key))
            {
                result[item.Key] = item.Value;
            }
        }

        return result;
    }
}