using InfluxDB.Client.Writes;

namespace WebLibServer.Metrics;

public interface IInfluxSvc
{
    void Write(PointData point);
}