using Xunit;
using WebAppShared.Security;

namespace WebAppSharedTest.Security
{
    public class SanitizeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("This is a plain text.")]
        [InlineData("<div unknown=\"tag\">Content</div>", "<div>Content</div>")]
        [InlineData("<div unknown=\"tag\" some=\"value\">Content</div>", "<div>Content</div>")]
        public void EditorText_GivenEmptyString_ReturnsEmptyString(string input, string? expected = null)
        {
            var result = Sanitize.EditorText(input);
            Assert.Equal(expected ?? input, result);
        }

        [Fact]
        public void EditorText_GivenHtmlString_SanitizesHtmlString()
        {
            var input = "<script>alert('hacked');</script>";
            var result = Sanitize.EditorText(input);
            Assert.DoesNotContain("<script>", result);
            Assert.DoesNotContain("</script>", result);
        }

        [Fact]
        public void EditorText_GivenHtmlWithAllowedAttributes_KeepsAllowedAttributes()
        {
            // To allow certain attributes in future, uncomment and use this test.
            /*
            var input = "<div class=\"example\" data-type=\"sample\">Content</div>";
            var result = Sanitize.EditorText(input);
            Assert.Contains("class=\"example\"", result);
            Assert.Contains("data-type=\"sample\"", result);
            */
        }

        [Fact]
        public void EditorText_GivenHtmlWithNotAllowedAttributes_RemovesNotAllowedAttributes()
        {
            // Since there are no allowed attributes in your example, all attributes should be stripped off.
            var input = "<div style=\"color:red\">Content</div>";
            var result = Sanitize.EditorText(input);
            Assert.DoesNotContain("style=\"color:red\"", result);
        }
    }
}
