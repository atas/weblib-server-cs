using WebAppShared.Extensions;

namespace WebAppSharedTest.Extensions
{
    public class ObjectExtensionTests
    {
        #region Tests for IsNullOrEmptyString
        [Theory]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("    ", true)]
        [InlineData("Hello, World!", false)]
        [InlineData(12345, false)]
        public void IsNullOrEmptyString_WithNullObject_ShouldReturnTrue(object data, bool expected)
        {
            bool result = data.IsNullOrEmptyString();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsNullOrEmptyString_WithNonStringObject_ShouldReturnFalse()
        {
            object obj = new DateTime(2023, 1, 1);
            bool result = obj.IsNullOrEmptyString();
            Assert.False(result);
        }
        #endregion
    }
}
