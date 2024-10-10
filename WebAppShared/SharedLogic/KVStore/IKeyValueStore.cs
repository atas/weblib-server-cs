namespace WebAppShared.SharedLogic.KVStore;

public interface IKeyValueStore
{
    string Get(KeyValueKeyBase key);
    void Save(KeyValueKeyBase key, string value);
}
