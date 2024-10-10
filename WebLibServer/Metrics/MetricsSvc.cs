using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using WebLibServer.SharedLogic.KVStore;
using WebLibServer.WebSys.DI;

namespace WebLibServer.Metrics;

[Service(InterfaceToBind = typeof(IMetricsSvc)), UsedImplicitly]
public class MetricsSvc(IInfluxSvc influxSvc) : IMetricsSvc
{
    public void Collect(string measurement, int value = 1, Dictionary<string, string> tags = null)
    {
        var pd = PointData.Measurement(measurement).Field("value", value);

        tags?.ForEach(tag => pd.Tag(tag.Key, tag.Value));

        influxSvc.Write(pd.Timestamp(DateTime.UtcNow, WritePrecision.S));
    }

    public void CollectNamed(BaseAppEvent measurement, int value = 1, Dictionary<string, string> tags = null)
    {
        Collect(measurement, value, tags);
    }
}