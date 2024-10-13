using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace WebLibServer.WebSys.MvcAttributes;

public class WebDomainAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var domain = value as string;
        if (string.IsNullOrWhiteSpace(domain))
        {
            return ValidationResult.Success;
        }

        var regex = new Regex(@"^[a-zA-Z0-9][a-zA-Z0-9-\.]*\.[a-zA-Z]{2,63}$");
        if (regex.IsMatch(domain))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult("The field is not a valid web domain name.");
    }
}
