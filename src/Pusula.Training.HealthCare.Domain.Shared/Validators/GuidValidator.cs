using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Validators;

public class GuidValidator : ValidationAttribute
{
    public GuidValidator() : base(HealthCareDomainErrorKeyValuePairs.GuidRequired.Value)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        if (value is Guid guidValue)
        {
            return guidValue == Guid.Empty
                ? new ValidationResult(ErrorMessage)
                : ValidationResult.Success;
        }

        return new ValidationResult(HealthCareDomainErrorKeyValuePairs.GuidNotValid.Value);
    }
}