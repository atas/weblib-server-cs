namespace WebAppShared.Security.RateLimiter;

public class RateLimit(int limit, TimeSpan timeSpan)
{
    public int Limit { get; set; } = limit;
    public TimeSpan TimeSpan { get; set; } = timeSpan;
}