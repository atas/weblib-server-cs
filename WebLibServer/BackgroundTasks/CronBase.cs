using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebLibServer.Metrics;

namespace WebLibServer.BackgroundTasks;

public abstract class CronBase(ILogger logger, IMetricsSvc metricsSvc) : BackgroundService
{
    protected abstract int DurationInMinutes { get; }

    protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(DurationInMinutes), stoppingToken);

            logger.LogInformation("Running Cron {Name}", GetType().Name);

            try
            {
                await RunAsync(stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                metricsSvc.CollectNamed(BaseAppEvent.CriticalException, 1, new Dictionary<string, string>
                {
                    {"Message", ex.Message},
                    {"type", "OperationCanceledException"}
                });

                logger.LogError(ex, "OperationCancelledException {Msg}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                metricsSvc.CollectNamed(BaseAppEvent.CriticalException, 1, new Dictionary<string, string>
                {
                    {"Message", ex.Message},
                    {"type", "Exception"}
                });
                logger.LogError(ex, "Exception while sending web push Notifications: {Msg}", ex.Message);
                throw;
            }
            finally
            {
                logger.LogInformation("Finished running Cron {Name}, will wait {Mins} minutes",
                    GetType().Name, DurationInMinutes);
            }
        }
    }

    protected abstract Task RunAsync(CancellationToken stoppingToken);
}