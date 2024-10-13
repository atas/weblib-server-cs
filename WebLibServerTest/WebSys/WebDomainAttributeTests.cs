using System.ComponentModel.DataAnnotations;
using WebLibServer.WebSys.MvcAttributes;

namespace WebLibServerTest.WebSys;

public class WebDomainAttributeTests
{
    private readonly WebDomainAttribute _attribute = new();

    [Fact]
    public void NullValue_ReturnsSuccess()
    {
        // Arrange
        object value = null;

        // Act
        var result = _attribute.GetValidationResult(value, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void EmptyString_ReturnsSuccess()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var result = _attribute.GetValidationResult(value, new ValidationContext(new object()));

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("www.example.com")]
    [InlineData("www.eXAmple.com")]
    [InlineData("www.example.ie")]
    [InlineData("www.exam-ple.ie")]
    [InlineData("example.uk.to")]
    [InlineData("www.example.uk.to")]
    public void ValidDomain_ReturnsSuccess(string value)
    {
        var result = _attribute.GetValidationResult(value, new ValidationContext(new object()));
        Assert.Equal(ValidationResult.Success, result);
    }

    [Theory]
    [InlineData("invalid_domain")]
    [InlineData("invalid_domain.")]
    [InlineData("invalid_domain.com.")]
    [InlineData("invalid_domain.com")]
    [InlineData("invalid,domain.com")]
    public void InvalidDomain_ReturnsValidationResult(string value)
    {
        var result = _attribute.GetValidationResult(value, new ValidationContext(new object()));
        Assert.NotEqual(ValidationResult.Success, result);
        Assert.Equal("The field is not a valid web domain name.", result.ErrorMessage);
    }
}