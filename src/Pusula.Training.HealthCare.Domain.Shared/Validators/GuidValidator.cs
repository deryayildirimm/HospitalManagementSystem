using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Validators;

public class GuidValidator : ValidationAttribute
{
    public GuidValidator() : base("The GUID field cannot be empty or default.")
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
            return guidValue == Guid.Empty || guidValue == default(Guid)
                ? new ValidationResult(ErrorMessage)
                : ValidationResult.Success;
        }

        return new ValidationResult("The provided value is not a valid GUID.");
    }
    
}