using System.Collections.Concurrent;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using WebAppShared.WebSys.DI;

namespace WebAppShared.SharedLogic.KVStore;

[UsedImplicitly]
public abstract class KeyValueStoreBase(ILogger<KeyValueStoreBase> logger) : IKeyValueStore
{
    private static DateTime _lastUpdatedAt = DateTime.MinValue;
    
    // TODO Make this below a ConcurrentDictionary AND adjust for tenants
    private static ConcurrentDictionary<string, string> _keyValues = new();

    private static readonly object Lock = new();

    protected abstract ConcurrentDictionary<string, string> GetAllFromDb();
    protected abstract void SaveToDb(KeyValueKeyBase key, string value);

    [CanBeNull]
    public string Get(KeyValueKeyBase key)
    {
        TryUpdateLocalKeyValues();

        // ReSharper disable once InconsistentlySynchronizedField
        return _keyValues[key];
    }

    private void TryUpdateLocalKeyValues()
    {
        if (DateTime.UtcNow.Subtract(_lastUpdatedAt) < TimeSpan.FromHours(1))
            return;

        lock (Lock)
        {
            if (DateTime.UtcNow.Subtract(_lastUpdatedAt) < TimeSpan.FromHours(1))
                return;
            UpdateLocalStore();
        }
    }

    private void UpdateLocalStore()
    {
        logger.LogInformation("Updating local KeyValue store");
        _keyValues = GetAllFromDb();
        _lastUpdatedAt = DateTime.UtcNow;
    }

    public void Save(KeyValueKeyBase key, string value)
    {
        lock (Lock)
        {
            logger.LogInformation("Updating KV {Key}={Value}", key, value);
            _keyValues[key] = value;
            
            SaveToDb(key, value);
        }
    }
}