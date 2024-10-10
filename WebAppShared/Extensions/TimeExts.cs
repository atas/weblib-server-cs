using System;

namespace WebAppShared.Extensions;

public static class TimeExts
{
	public static long? ToTryEpochTime(this DateTime? dateTime)
	{
		if (dateTime == DateTime.MinValue || dateTime == DateTime.MaxValue || dateTime == default(DateTime))
			return 0;
		
		return dateTime == null ? (long?)null : ToTimestamp(dateTime.Value);
	}

	/// <summary>
	/// Converts the UTC given date value to timestamp.
	/// </summary>
	public static long ToTimestamp(this DateTime utcDateTime)
	{
		if (utcDateTime == DateTime.MinValue || utcDateTime == DateTime.MaxValue || utcDateTime == default)
			return 0;
		
		DateTimeOffset dateTimeOffset = new DateTimeOffset(utcDateTime);
		return dateTimeOffset.ToUnixTimeSeconds();

		// // var date = dateTime.ToUniversalTime(); //everything we use in db are utc so we consider datetime as utc
		// var ticks = dateTime.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
		// var ts = ticks / TimeSpan.TicksPerSecond;
		// return ts;
	}

	/// <summary>
	/// Converts the UTC given date value to epoch time.
	/// </summary>
	public static long ToTimestamp(this DateTimeOffset dateTimeOffset)
	{
		dateTimeOffset.ThrowIfNotUtc();
		
		if (dateTimeOffset.UtcDateTime == DateTime.MinValue || dateTimeOffset.UtcDateTime == DateTime.MaxValue || dateTimeOffset == default)
			return 0;
		
		return dateTimeOffset.ToUnixTimeSeconds();
	}

	/// <summary>
	/// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
	/// </summary>
	public static DateTime ToDateTime(this long intDate)
	{
		if (intDate < 0) throw new ArgumentOutOfRangeException(nameof(intDate), "Timestamp cannot be negative");

		var val = DateTime.UnixEpoch.AddSeconds(intDate);
		val.ThrowIfNotUtc();

		return val;
	}

	/// <summary>
	/// Converts the given epoch time to a UTC <see cref="DateTimeOffset"/>.
	/// </summary>
	public static DateTimeOffset ToDateTimeOffset(this long intDate)
	{
		if (intDate < 0) throw new ArgumentOutOfRangeException(nameof(intDate), "Timestamp cannot be negative");

		return DateTimeOffset.FromUnixTimeSeconds(intDate);
	}

	public static bool IsUtc(this DateTime dateTime)
	{
		return dateTime.Kind == DateTimeKind.Utc;
	}

	public static void ThrowIfNotUtc(this DateTime dateTime)
	{
		if (!dateTime.IsUtc())
			throw new ArgumentException("DateTime must be UTC", nameof(dateTime));
	}

	public static void ThrowIfNotUtc(this DateTimeOffset dateTimeOffset)
	{
		if (dateTimeOffset.Offset != TimeSpan.Zero)
			throw new ArgumentException("DateTimeOffset must have an offset of zero (UTC).", nameof(dateTimeOffset));
	}

}
