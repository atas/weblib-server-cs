﻿using InfluxDB.Client;
using InfluxDB.Client.Writes;
using JetBrains.Annotations;
using WebAppShared.Config;
using WebAppShared.WebSys.DI;

namespace WebAppShared.Metrics;

[Service(InterfaceToBind = typeof(IInfluxSvc)), UsedImplicitly]
public class InfluxSvc(InfluxConfig influxConfig) : IInfluxSvc
{
    public void Write(PointData point)
    {
        var options = new InfluxDBClientOptions.Builder()
            .Url(influxConfig.Url)
            .AuthenticateToken(influxConfig.Token.ToCharArray())
            .Bucket(influxConfig.Bucket)
            .Org(influxConfig.Org)
            .Build();
        
        using var client = new InfluxDBClient(options);
        
        using var writeApi = client.GetWriteApi();

        writeApi.WritePoint(point);
    }
}
