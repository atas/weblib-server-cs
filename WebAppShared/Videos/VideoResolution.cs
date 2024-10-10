using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebAppShared.Videos;

public class VideoResolution
{
	public static VideoResolution X5K = new("5K", 5120, 2880);
	public static VideoResolution X4K = new("4K", 3840, 2160);
	public static VideoResolution X2160P = new("2160p", 3840, 2160);
	public static VideoResolution X1440P = new("1440p", 2560, 1440);
	public static VideoResolution X1080P = new("1080p", 1920, 1080);
	public static VideoResolution X720P = new("720p", 1280, 720);
	public static VideoResolution X480P = new("480p", 852, 480);
	public static VideoResolution X360P = new("360p", 480, 360);
	public static VideoResolution XLow = new("Low", 0, 0);

	public string Name { get; }

	public int Width { get; }

	public int Height { get; }

	private static Dictionary<string, VideoResolution> _resolutions;
	private static List<VideoResolution> _list;

	public VideoResolution(string name, int width, int height)
	{
		Name = name;
		Width = width;
		Height = height;
	}

	/// <summary>
	/// Returns the list of video resolutions as (name,object) dictionary pairs.
	/// </summary>
	/// <returns></returns>
	public static Dictionary<string, VideoResolution> GetDictionary()
	{
		if (_resolutions != null) return _resolutions;

		var props = typeof(VideoResolution).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField);

		var map = new Dictionary<string, VideoResolution>();

		foreach (var p in props)
		{
			if (p.FieldType == typeof(VideoResolution) && p.Name.StartsWith("X"))
			{
				var val = (VideoResolution)p.GetValue(typeof(VideoResolution));
				if (val != null)
					map.Add(val.Name, val);
			}
		}

		_resolutions = map;

		return map;
	}

	public static List<VideoResolution> GetList() =>
		_list ??= GetDictionary().Values.ToList().OrderByDescending(r => r.Height).ToList();

	public static VideoResolution GetByDimension(int width, int height)
	{
		foreach (var resolution in GetList())
		{
			if (width >= resolution.Width || height >= resolution.Height)
				return resolution;
		}

		return XLow;
	}

	public static VideoResolution GetByName(string name)
	{
		return GetDictionary()[name];
	}

	public static implicit operator VideoResolution(string value) => GetDictionary()[value];
	public static implicit operator string(VideoResolution value) => value.Name;
	public override string ToString() => Name;
}
