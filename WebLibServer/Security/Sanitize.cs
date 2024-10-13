using Ganss.Xss;
using MoreLinq;

namespace WebLibServer.Security;

public static class Sanitize
{
	private static readonly string[] AllowedAttrs = { }; //{ "class", "data-type", "data-value", "data-id", "data-denotation-char" };

	public static string EditorText(string txt)
	{
		var sanitizer = new HtmlSanitizer();
		AllowedAttrs.ForEach(a => sanitizer.AllowedAttributes.Add(a));
		return sanitizer.Sanitize(txt);
	}
}
