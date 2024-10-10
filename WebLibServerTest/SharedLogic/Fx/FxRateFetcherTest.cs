using WebLibServer.SharedLogic.FxConverter;
using WebLibServerTest.TestLibs;
using Xunit.Abstractions;

namespace WebLibServerTest.SharedLogic.Fx;

public class FxRateFetcherTest(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public Task TestFxRateFetcherWorks()
    {
        var fxRateFetcher = new FxRateFetcher(output.BuildLoggerFor<FxRateFetcher>());

        var result = fxRateFetcher.Fetch();

        Assert.True(result.fromDate > DateTime.UtcNow - TimeSpan.FromDays(7));
        Assert.True(result.rates.Count > 30);
        return Task.CompletedTask;
    }
}