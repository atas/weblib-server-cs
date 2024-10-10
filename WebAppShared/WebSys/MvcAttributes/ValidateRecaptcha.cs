using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using WebAppShared.Exceptions;
using WebAppShared.SharedLogic.Recaptcha;

namespace WebAppShared.WebSys.MvcAttributes;

/// <summary>
///     Validation attribute for validating recaptcha
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidateRecaptcha(string keyName = "recaptchaValue", bool throwIfFail = true) : ValidationAttribute
{
    private readonly string _keyName = keyName;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var hca = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
        var recaptchaValidator = (RecaptchaValidator)validationContext.GetService(typeof(RecaptchaValidator));

        if (hca == null || recaptchaValidator == null)
            return Fail("Could not obtain recaptcha validation dependencies");

        if (!recaptchaValidator.Validate((string)value).Result)
            return Fail("Captcha wasn't valid, please re-do the security check.");

        return ValidationResult.Success;
    }

    private ValidationResult Fail(string msg)
    {
        if (throwIfFail)
            throw new HttpJsonError(msg);

        return new ValidationResult(msg);
    }
}