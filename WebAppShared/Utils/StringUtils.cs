using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace WebAppShared.Utils;

public static class StringUtils
{
	private static readonly Random Random = new Random();

	public static string RandomString(int length)
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[Random.Next(s.Length)]).ToArray());
	}

	public static string Shorten(this string value, int maxChars)
	{
		if (value == null) return null;
		value = Regex.Replace(value.Trim(), @"\s+", " ").Trim();
		return value.Length <= maxChars ? value : value.Substring(0, maxChars).Trim() + "...";
	}

	public static string ToBase64(this string txt)
	{
		byte[] encodedBytes = Encoding.Unicode.GetBytes(txt);
		return Convert.ToBase64String(encodedBytes);
	}

	public static string FromBase64(this string txt)
	{
		var bytes = Convert.FromBase64String(txt);
		return Encoding.Unicode.GetString(bytes);
	}

	public static string TrimAndNullIfEmpty([CanBeNull] this string str)
	{
		return String.IsNullOrWhiteSpace(str) ? null : str.Trim();
	}

	public static string StripHtmlTags(this string input) => Regex.Replace(input, "<.*?>", String.Empty);
	
	public static string FirstLetterToUpper(string str)
	{
		if (str == null)
			return null;

		if (str.Length > 1)
			return char.ToUpper(str[0]) + str.Substring(1);

		return str.ToUpper();
	}
}
