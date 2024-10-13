using Microsoft.Extensions.Logging;
using Moq;
using WebLibServer.Contexts;
using WebLibServer.Exceptions;
using WebLibServer.Security.RateLimiter;

namespace WebLibServerTest.Security.RateLimiter;

public class RateLimiterTests
{
    [Fact]
    public void EvaluateTimestamps_WhenUnderLimit_NoExceptionThrown()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockConnectionCx = new Mock<IConnectionCx>();
        mockConnectionCx.Setup(cx => cx.IpAddress).Returns("127.0.0.1");
        var rateLimiter = new TestableRateLimiter(mockConnectionCx.Object, mockLogger.Object,
            [new RateLimit(5, TimeSpan.FromMinutes(1))]);

        var timestamps = new List<DateTime> { DateTime.UtcNow.AddSeconds(-10) }; // 1 timestamp 10 seconds ago

        // Act & Assert
        var exception = Record.Exception(() => rateLimiter.EvaluateTimestamps(timestamps));
        Assert.Null(exception);
    }

    [Fact]
    public void EvaluateTimestamps_WhenOverLimit_ExceptionThrown()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockConnectionCx = new Mock<IConnectionCx>();
        mockConnectionCx.Setup(cx => cx.IpAddress).Returns("127.0.0.1");
        var rateLimiter = new TestableRateLimiter(mockConnectionCx.Object, mockLogger.Object,
            [new(3, TimeSpan.FromMinutes(1))]);

        var timestamps = new List<DateTime>
        {
            DateTime.UtcNow.AddSeconds(-10),
            DateTime.UtcNow.AddSeconds(-20),
            DateTime.UtcNow.AddSeconds(-30),
            DateTime.UtcNow.AddSeconds(-40) // 4 timestamps within the last minute
        };

        // Act & Assert
        var exception = Assert.Throws<HttpJsonError>(() => rateLimiter.EvaluateTimestamps(timestamps));
        Assert.Equal("Rate limit exceeded. Try again later.", exception.Title);
    }

    [Fact]
    public void OldestRateLimitTime_CalculatesCorrectTime()
    {
        // Arrange
        var mockLogger = new Mock<ILogger>();
        var mockConnectionCx = new Mock<IConnectionCx>();
        var rateLimiter = new TestableRateLimiter(mockConnectionCx.Object, mockLogger.Object, [
            new(5, TimeSpan.FromHours(1)),
            new(10, TimeSpan.FromHours(2))
        ]);

        var expected = DateTime.UtcNow - TimeSpan.FromHours(2);

        // Act
        var actual = rateLimiter.OldestRateLimitTime();

        // Assert
        Assert.True((expected - actual) < TimeSpan.FromSeconds(1)); // Allowing a small leeway
    }
}