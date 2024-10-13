using WebLibServer.Images;

namespace WebLibServerTest.Images;

public class MaxImageSizeWithRatioTest
{
    [Theory]
    [InlineData(100, 200, 50, 25, 50)]
    [InlineData(200, 100, 50, 50, 25)]
    [InlineData(100, 100, 50, 50, 50)]
    [InlineData(650, 350, 150, 150, 80)]
    public void MaxImageSizeByRatioTester(int fullWidth, int fullHeight, int maxWidthOrHeight, int resultWidth,
        int resultHeight)
    {
        var ratio = new MaxImageSizeByRatio(fullWidth, fullHeight, maxWidthOrHeight);

        Assert.Equal(resultWidth, ratio.Width);
        Assert.Equal(resultHeight, ratio.Height);
    }
}