using InfluxDB.Client.Writes;

namespace WebAppShared.Metrics;

public interface IInfluxSvc
{
    void Write(PointData point);
}