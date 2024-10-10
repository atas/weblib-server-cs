using WebAppShared.SharedLogic.FxConverter;
using WebAppSharedTest.TestLibs;
using Xunit.Abstractions;

namespace WebAppSharedTest.SharedLogic.Fx;

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