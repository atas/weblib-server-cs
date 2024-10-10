namespace WebAppShared.Images;

/// <summary>
/// Calculates the maximum image size by ratio.
/// </summary>
/// <param name="fullWidth"></param>
/// <param name="fullHeight"></param>
/// <param name="maxWidthOrHeight"></param>
public class MaxImageSizeByRatio(int fullWidth, int fullHeight, int maxWidthOrHeight)
{
	private readonly double _ratio = fullWidth / (double)fullHeight;

	private int? _calculatedWidth;
	private int? _calculatedHeight;

	public int Width => _calculatedWidth ?? CalculateWidth();
	public int Height => _calculatedHeight ?? CalculateHeight();

	private int CalculateWidth()
	{
		_calculatedWidth = _ratio < 1 ? (int)(_ratio * maxWidthOrHeight) : maxWidthOrHeight;
		return _calculatedWidth.Value;
	}

	private int CalculateHeight()
	{
		_calculatedHeight = _ratio > 1 ? (int)(1 / _ratio * maxWidthOrHeight) : maxWidthOrHeight;
		return _calculatedHeight.Value;
	}
}
