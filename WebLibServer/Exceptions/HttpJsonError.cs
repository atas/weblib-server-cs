using System;

namespace WebLibServer.Exceptions;

/// <summary>
/// The user-error returned from APIs, returned manually by throwing
/// </summary>
public class HttpJsonError(string title) : Exception
{
	public string Title { get; set; } = title;
	public string[] Errors { get; set; }
}
