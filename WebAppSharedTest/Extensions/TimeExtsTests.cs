using WebAppShared.Extensions;

namespace WebAppSharedTest.Extensions
{
    public class TimeExtsTests
    {
        #region Tests for ToTimestamp(DateTime)
        [Fact]
        public void TestToTimestampForEpochStart()
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = dateTime.ToTimestamp();

            Assert.Equal(0L, timestamp);
        }

        [Fact]
        public void TestToTimestampForKnownDate()
        {
            var dateTime = new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = dateTime.ToTimestamp();

            Assert.Equal(946684800L, timestamp); // UNIX timestamp for 2000-01-01 00:00:00 UTC
        }

        [Fact]
        public void TestToTimestampForRecentDate()
        {
            var dateTime = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = dateTime.ToTimestamp();

            Assert.Equal(1672531200L, timestamp); // UNIX timestamp for 2023-01-01 00:00:00 UTC
        }
        #endregion

        #region Tests for IsUtc()
        [Fact]
        public void TestIsUtc()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            DateTime localDateTime = DateTime.Now;

            Assert.True(utcDateTime.IsUtc());
            Assert.False(localDateTime.IsUtc());
        }
        #endregion

        #region Tests for ThrowIfNotUtc()
        [Fact]
        public void ThrowIfNotUtc_ThrowsWhenDateTimeIsLocal()
        {
            DateTime localDateTime = DateTime.Now;
            Assert.Throws<ArgumentException>(() => localDateTime.ThrowIfNotUtc());
        }

        [Fact]
        public void ThrowIfNotUtc_ThrowsWhenDateTimeIsUnspecified()
        {
            DateTime unspecifiedDateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            Assert.Throws<ArgumentException>(() => unspecifiedDateTime.ThrowIfNotUtc());
        }
        #endregion

        #region Tests for ThrowIfNotUtc(DateTime)
        [Fact]
        public void ThrowIfNotUtc_DoesNotThrowWhenDateTimeIsUtc()
        {
            DateTime utcDateTime = DateTime.UtcNow;
            var exception = Record.Exception(() => utcDateTime.ThrowIfNotUtc());
            Assert.Null(exception);
        }

        #endregion

        #region Tests for ToTimestamp(DateTimeOffset)
        [Fact]
        public void ToTimestamp_ForEpochStart_ShouldReturnZero()
        {
            var dateTimeOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var timestamp = dateTimeOffset.ToTimestamp();

            Assert.Equal(0L, timestamp);
        }

        [Fact]
        public void ToTimestamp_ForKnownDate_ShouldReturnExpectedValue()
        {
            var dateTimeOffset = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var timestamp = dateTimeOffset.ToTimestamp();

            Assert.Equal(946684800L, timestamp); // UNIX timestamp for 2000-01-01 00:00:00 UTC
        }

        [Fact]
        public void ToTimestamp_ForRecentDate_ShouldReturnExpectedValue()
        {
            var dateTimeOffset = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var timestamp = dateTimeOffset.ToTimestamp();

            Assert.Equal(1672531200L, timestamp); // UNIX timestamp for 2023-01-01 00:00:00 UTC
        }

        [Fact]
        public void ToTimestamp_ForDateTimeWithNonZeroOffset_ShouldThrowArgumentException()
        {
            // 5 hours ahead of UTC
            var dateTimeOffset = new DateTimeOffset(2000, 1, 1, 5, 0, 0, TimeSpan.FromHours(5));

            // Expect an ArgumentException because of the non-zero offset
            Assert.Throws<ArgumentException>(() => dateTimeOffset.ToTimestamp());
        }

        #endregion

        #region Tests for ToDateTime(long)
        [Theory]
        [InlineData(0, 1970, 1, 1, 0, 0, 0)]
        [InlineData(1609459200, 2021, 1, 1, 0, 0, 0)] // 2021-01-01T00:00:00Z
        [InlineData(1614556800, 2021, 3, 1, 0, 0, 0)] // 2021-03-01T00:00:00Z
        [InlineData(1622520000, 2021, 6, 1, 4, 0, 0)] // 2021-06-02T01:00:00Z
        public void ToDateTime_ShouldConvertEpochToUtcDateTime(long epoch, int year, int month, int day, int hour,
            int minute, int second)
        {
            // Act
            DateTime result = epoch.ToDateTime();

            Assert.Equal(year, result.Year);
            Assert.Equal(month, result.Month);
            Assert.Equal(day, result.Day);
            Assert.Equal(hour, result.Hour);
            Assert.Equal(minute, result.Minute);
            Assert.Equal(second, result.Second);
            Assert.Equal(DateTimeKind.Utc, result.Kind);
        }

        [Fact]
        public void ToDateTime_NegativeEpoch_ShouldThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => (-1L).ToDateTime());
        }

        [Fact]
        public void ToDateTime_TooLargeEpoch_ShouldThrowArgumentOutOfRangeException()
        {
            // The maximum DateTime value is around year 10000.
            // An epoch time significantly larger than the current time (e.g., for year 15000) will cause an overflow.
            Assert.Throws<ArgumentOutOfRangeException>(() => 500000000000L.ToDateTime());
        }
        #endregion

        #region Tests for ToDateTimeOffset(long)
        [Theory]
        [InlineData(0, 1970, 1, 1, 0, 0, 0)]
        [InlineData(1609459200, 2021, 1, 1, 0, 0, 0)] // 2021-01-01T00:00:00Z
        [InlineData(1614556800, 2021, 3, 1, 0, 0, 0)] // 2021-03-01T00:00:00Z
        [InlineData(1622520000, 2021, 6, 1, 4, 0, 0)] // 2021-06-02T01:00:00Z
        public void ToDateTimeOffset_ShouldConvertEpochToCorrectDateTimeOffset(long epoch, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            DateTimeOffset result = epoch.ToDateTimeOffset();

            // Assert
            Assert.Equal(year, result.Year);
            Assert.Equal(month, result.Month);
            Assert.Equal(day, result.Day);
            Assert.Equal(hour, result.Hour);
            Assert.Equal(minute, result.Minute);
            Assert.Equal(second, result.Second);
            Assert.Equal(TimeSpan.Zero, result.Offset);
        }

        [Fact]
        public void ToDateTimeOffset_NegativeEpoch_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => (-1L).ToDateTimeOffset());
        }

        [Fact]
        public void ToDateTimeOffset_TooLargeEpoch_ShouldThrowArgumentOutOfRangeException()
        {
            // Act & Assert
            // The maximum DateTimeOffset value is around year 10000.
            // An epoch time significantly larger than the current time (e.g., for year 15000) will cause an overflow.
            Assert.Throws<ArgumentOutOfRangeException>(() => 500000000000L.ToDateTimeOffset());
        }
        #endregion
    }
}