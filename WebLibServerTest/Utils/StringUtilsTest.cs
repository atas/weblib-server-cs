using WebLibServer.Utils;

namespace WebLibServerTest.Utils
{
    public class StringUtilsTest
    {
        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(20)]
        public void RandomString_ShouldReturnStringOfSpecifiedLength(int length)
        {
            var map = new HashSet<string>();

            // loop above code 100 times
            for (int i = 0; i < 10; i++)
            {
                var result = StringUtils.RandomString(length);
                Assert.DoesNotContain(result, map);
                Assert.Equal(length, result.Length);
            }
        }


        [Fact]
        public void RandomString_ShouldOnlyContainValidChars()
        {
            string result = StringUtils.RandomString(100);
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Assert.All(result, c => Assert.Contains(c, validChars));
        }

        [Theory]
        [InlineData("short", 10, "short")]
        [InlineData("this is a long string", 10, "this is a...")]
        [InlineData(null, 10, null)]
        [InlineData("", 10, "")]
        [InlineData("        ", 10, "")]
        [InlineData("some             value", 3, "som...")]
        public void Shorten_ShouldReturnShortenedOrUnchangedString(string input, int maxLength, string expected)
        {
            Assert.Equal(expected, input.Shorten(maxLength));
        }

        [Theory]
        [InlineData("test string")]
        [InlineData("")]
        [InlineData("!@#$%^&*()_-+=[]{}|;:'\",.<>?/")]
        [InlineData("你好世界")] // "Hello World" in Chinese
        [InlineData("test\nstring")]
        public void ToBase64AndFromBase64_ShouldBeReversible(string original)
        {
            string encoded = original.ToBase64();
            string decoded = encoded.FromBase64();
            Assert.Equal(original, decoded);
        }

        [Fact]
        public void FromBase64_WithInvalidBase64_ShouldThrowFormatException()
        {
            string invalidBase64 = "!@#$%^&"; // This is not a valid Base64 string.
            Assert.Throws<FormatException>(() => invalidBase64.FromBase64());
        }

        [Theory]
        [InlineData("Hello World", "SABlAGwAbABvACAAVwBvAHIAbABkAA==")]
        [InlineData("OpenAI", "TwBwAGUAbgBBAEkA")]
        [InlineData("!@#$%^&*()", "IQBAACMAJAAlAF4AJgAqACgAKQA=")]
        public void ToBase64_WithFixedData_ShouldReturnExpectedBase64(string original, string expectedBase64)
        {
            string encoded = original.ToBase64();
            Assert.Equal(expectedBase64, encoded);
            Assert.Equal(original, encoded.FromBase64());
        }

        [Theory]
        [InlineData("   ", null)]
        [InlineData("test", "test")]
        [InlineData(null, null)]
        public void TrimAndNullIfEmpty_ShouldTrimAndNullifyIfEmpty(string input, string expected)
        {
            Assert.Equal(expected, input.TrimAndNullIfEmpty());
        }

        [Theory]
        [InlineData("<p>This is a paragraph.</p>", "This is a paragraph.")]
        [InlineData("<div><h1>Title</h1></div>", "Title")]
        [InlineData("<div script='alert(\"hi\")'><h1>Title</h1></div>", "Title")]
        public void StripHtmlTags_ShouldRemoveHtmlTags(string input, string expected)
        {
            Assert.Equal(expected, input.StripHtmlTags());
        }


        [Theory]
        [InlineData("test", "Test")]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("Test", "Test")]
        [InlineData("t", "T")]
        [InlineData("1test", "1test")]
        public void FirstLetterToUpper(string input, string expected)
        {
            var result = StringUtils.FirstLetterToUpper(input);
            Assert.Equal(expected, result);
        }
    }
}
