using Moq;
using WebAppShared.SharedLogic.Fx;
using WebAppShared.SharedLogic.FxConverter;
using WebAppSharedTest.TestLibs;
using Xunit.Abstractions;

namespace WebAppSharedTest.SharedLogic.Fx;

public class FxConverterTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestFxConverter()
    {
        var fxRateSvc = new Mock<FxRateSvc>(null, null, null);
        fxRateSvc.Setup(c => c.GetRate(Currency.USD)).Returns(1.1M);
        fxRateSvc.Setup(c => c.GetRate(Currency.EUR)).Returns(1M);

        var converter = new FxConverter(fxRateSvc.Object);

        var result = converter.Convert(1, Currency.EUR, Currency.USD);
        Assert.Equal(1.1M, result);

        var result2 = converter.Convert(1, Currency.USD, Currency.EUR);

        Assert.Equal(0.91M, Math.Round(result2, 2));
    }
}
