using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Pusula.Training.HealthCare.Validators;

public class CreatePhoneNumberValidator : ValidationAttribute
{
    private const string PhoneNumberPattern = @"^\d{5,10}$";

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var displayName = validationContext.DisplayName;
        var stringValue = value?.ToString();

        if (string.IsNullOrEmpty(stringValue))
        {
            return ValidationResult.Success!;
        }

        var regex = new Regex(PhoneNumberPattern);

        if (regex.IsMatch(stringValue!))
        {
            return ValidationResult.Success!;
        }

        var formattedDisplayName = InsertSpaces(displayName);
        return new ValidationResult($"Please enter a valid {formattedDisplayName}.");
    }
    private string InsertSpaces(string input)
    {
        var result = string.Concat(input.Select((x, i) => i > 0 && Char.IsUpper(x) ? " " + x : x.ToString()));
        return result;
    }
}