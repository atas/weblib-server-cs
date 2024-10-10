using WebAppShared.Types;

namespace WebAppShared.SharedLogic.KVStore;

public class KeyValueKeyBase(string value) : EnumClass<string>(value)
{
    public static KeyValueKeyBase FxFetchedAt { get; set; } = new("FxFetchedAt");
    public static KeyValueKeyBase FxFromAt { get; set; } = new("FxFromAt");
    public static KeyValueKeyBase Fx { get; set; } = new("Fx");
}