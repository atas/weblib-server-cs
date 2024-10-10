using WebLibServer.Http;

namespace WebLibServerTest;

public class TestUrlUtils
{
	[Theory]
	[InlineData("https://www.google.com/dsfggdf?xxxx=32434", "google.com/dsfggdf?xxxx=32434")]
	[InlineData("https://www.google.com/", "google.com/")]
	[InlineData("https://www.google.com", "google.com/")]
	[InlineData("https://www.google.com/?xxxx=32434", "google.com/?xxxx=32434")]
	public void TestBaseDomainUrl(string url, string expected)
	{
		var urlWithBaseDomain = UrlUtils.GetUrlWithBaseDomain(url);

		Assert.Equal(expected, urlWithBaseDomain);
	}

	[Theory]
	[InlineData("Girls rejects guy in front of people", "girls-rejects-guy-in-front-of-people")]
	[InlineData("Tate - Money doesn't make you happy by", "tate-money-doesnt-make-you-happy-by")]
	[InlineData("Tate      Money doesn't make you happy        by", "tate-money-doesnt-make-you-happy-by")]
	public void SlugTest(string title, string expected)
	{
		var converted = UrlUtils.ConvertToSlug(title);

		Assert.Equal(expected, converted);
	}
}
