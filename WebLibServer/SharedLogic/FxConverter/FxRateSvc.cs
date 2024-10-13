using Google.Apis.Util;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebLibServer.Exceptions;
using WebLibServer.Extensions;
using WebLibServer.SharedLogic.Fx;
using WebLibServer.SharedLogic.KVStore;
using WebLibServer.Types;
using WebLibServer.WebSys.DI;

namespace WebLibServer.SharedLogic.FxConverter;

/// <summary>
///     Responsible of providing FX values to other components of the system
/// </summary>
[Service, UsedImplicitly]
public class FxRateSvc(IKeyValueStore keyValueStore, FxRateFetcher fxRateFetcher, ILogger<FxRateSvc> logger)
{
    private readonly object _lock = new();

    private Dictionary<string, decimal?> _fxDict = new();
    private DateTime? _fxFetchedAt;
    private DateTime? _fxFromAt;

    public virtual decimal GetRate(Currency currency)
    {
        currency.ThrowIfNull(nameof(currency));

        TryUpdateFx();

        if (currency == Currency.EUR) return 1;

        var currByEuro = _fxDict[currency];

        if (currByEuro == null)
            throw new HttpJsonError($"Exchange rate could not found for {currency}");

        return currByEuro.Value;
    }

    private void TryUpdateFx()
    {
        if (_fxFetchedAt != null && DateTime.UtcNow - _fxFetchedAt < TimeSpan.FromHours(3))
        {
            logger.LogInformation("Using existing Fx rates, not fetching and not updating");
            return;
        }

        lock (_lock)
        {
            // We just parsed this in another request, don't re-parse
            if (_fxFetchedAt != null && DateTime.UtcNow - _fxFetchedAt < TimeSpan.FromHours(3))
            {
                logger.LogInformation("Using existing Fx rates, not fetching and not updating");
                return;
            }

            var fetchResult = fxRateFetcher.Fetch();

            if (fetchResult.fromDate != _fxFromAt)
            {
                logger.LogInformation("Saving new FX rates to key value store");
                keyValueStore.Save(KeyValueKeyBase.FxFetchedAt, DateTime.UtcNow.ToTimestamp().ToString());
                keyValueStore.Save(KeyValueKeyBase.FxFromAt, fetchResult.fromDate.ToTimestamp().ToString());
                keyValueStore.Save(KeyValueKeyBase.Fx, JsonConvert.SerializeObject(fetchResult.rates));

                UpdateFromKvStore();
            }
            else
            {
                logger.LogInformation("No new FX rates are published yet by ECB, maybe tomorrow");
            }
        }
    }

    private void UpdateFromKvStore()
    {
        logger.LogInformation("Updating FX rates from the KV store");

        var fxStr = keyValueStore.Get(KeyValueKeyBase.Fx);

        var fxFetchedAtStr = keyValueStore.Get(KeyValueKeyBase.FxFetchedAt);
        DateTime? fxFetchedAt = fxFetchedAtStr == null ? null : long.Parse(fxFetchedAtStr).ToDateTime();
        _fxFetchedAt = fxFetchedAt;

        var fxFromAtStr = keyValueStore.Get(KeyValueKeyBase.FxFromAt);
        _fxFromAt = fxFromAtStr == null ? null : long.Parse(fxFromAtStr).ToDateTime();

        var fxDict = JsonConvert.DeserializeObject<Dictionary<string, decimal?>>(fxStr);
        _fxDict = fxDict;
    }
}