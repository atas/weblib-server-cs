namespace WebLibServer.Metrics;

public interface IMetricsSvc
{
    void Collect(string measurement, int value = 1, Dictionary<string, string> tags = null);
    void CollectNamed(BaseAppEvent measurement, int value = 1, Dictionary<string, string> tags = null);
}
