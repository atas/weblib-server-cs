using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebAppShared.WebSys.MvcAttributes;

/// <summary>
/// Validation attribute to assert a max size for a file
/// Should be used on IFormFile or IFormFileCollection
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class MaxFileSizeAttribute(int maxSizeInMb = 5) : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        List<IFormFile> formFiles = null;

        if (value is IFormFileCollection formCollection)
            formFiles = formCollection.ToList();
        else if (value is IFormFile formFile) formFiles = new List<IFormFile>(new[] { formFile });

        if (formFiles == null || formFiles.Count == 0)
            throw new Exception(
                $"Form file is of unknown type and cannot validate. Type: {value.GetType()} toString: {value}");

        return formFiles.All(IsFormFileAcceptableSize)
            ? ValidationResult.Success
            : new ValidationResult($"Max filesize is {maxSizeInMb}mb");
    }

    private bool IsFormFileAcceptableSize(IFormFile formFile)
    {
        return formFile.Length <= maxSizeInMb * 1024 * 1024;
    }
}