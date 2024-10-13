using System.Net;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using WebLibServer.Exceptions;

namespace WebLibServer.Photos;

public static class CommonPhotoUtils
{
	/// <summary>
	/// Gets ImageSharp encoder for a given extension
	/// </summary>
	/// <param name="ext"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	public static IImageEncoder GetEncoder(string ext)
	{
		ext = ext.ToLowerInvariant();

		if (ext is "jpg" or "jpeg")
			return new JpegEncoder();
		if (ext == "gif")
			return new GifEncoder();
		if (ext == "png")
			return new PngEncoder();
		if (ext == "webp")
			return new WebpEncoder();

		throw new Exception($"Can't find an encoder for the given photo file extension {ext}");
	}

	public static string GetMimeTypeByExt(string ext)
	{
		return ext.ToLowerInvariant() switch
		{
			"png" => "image/png",
			"jpg" => "image/jpeg",
			"jpeg" => "image/jpeg",
			"gif" => "image/gif",
			"webp" => "image/webp",
			_ => throw new HttpJsonError("Unsupported image extension")
		};
	}

	private static readonly string[] AllowedExtensions = { "png", "jpg", "jpeg", "gif", "webp" };
	public static readonly int[] AllowedSizes = { 32, 64, 128, 256, 512 };

	public static void AssertPhotoSizeValid(int? size)
	{
		if (size == null) return;

		if (!AllowedSizes.Contains(size.Value)) throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
	}

	/// <summary>
	/// Checks if the path/file extension is a photo/image file extension
	/// </summary>
	/// <param name="fullFileName"></param>
	/// <returns></returns>
	public static bool IsExtensionPhotoFile(string fullFileName)
	{
		if (fullFileName == null) return false;

		var ext = fullFileName.Split(".").Last().ToLowerInvariant();

		return AllowedExtensions.Contains(ext);
	}

	/// <summary>
	/// Checks if the given extension is an allowed photo extension.
	/// </summary>
	/// <param name="ext"></param>
	/// <returns></returns>
	public static bool IsExtensionAllowed(string ext) => AllowedExtensions.Any(e => e == ext);
}
