using System;

namespace WebLibServer.Photos;

public class PhotoFileUploaderResult
{
	public string Ext { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public string FullFileName { get; set; }

	/// <summary>
	/// i.e. Filename without extension
	/// </summary>
	public Guid Id { get; set; }
}
