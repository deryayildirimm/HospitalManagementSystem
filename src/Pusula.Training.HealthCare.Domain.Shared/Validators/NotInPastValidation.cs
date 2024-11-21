using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Validators;

public class NotInPastAttribute() : ValidationAttribute("Start date cannot be in the past.")
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var startDate = (DateTime)value;

        return startDate < DateTime.Now.Date
            ? new ValidationResult("Start date cannot be in the past.")
            : ValidationResult.Success;
    }
}