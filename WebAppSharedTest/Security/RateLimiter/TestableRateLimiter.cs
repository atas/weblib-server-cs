using Microsoft.Extensions.Logging;
using WebAppShared.Contexts;
using WebAppShared.Security.RateLimiter;

namespace WebAppSharedTest.Security.RateLimiter;

public class TestableRateLimiter(IConnectionCx connectionCx, ILogger logger, List<RateLimit> rateLimits)
    : RateLimiterBase(connectionCx, logger)
{
    protected override List<RateLimit> RateLimits { get; } = rateLimits;

    public override void Run()
    {
        throw new NotImplementedException();
    }
    
    public new void EvaluateTimestamps(List<DateTime> timestamps)
    {
        base.EvaluateTimestamps(timestamps);
    }
    
    public new DateTime OldestRateLimitTime()
    {
        return base.OldestRateLimitTime();
    }
}