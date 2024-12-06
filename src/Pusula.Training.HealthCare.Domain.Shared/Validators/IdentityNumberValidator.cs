using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Validators;

public class IdentityNumberValidator : ValidationAttribute
{
    private readonly int _minLength;

    public IdentityNumberValidator(int minLength = PatientConsts.PassportNumberMinLength)
    {
        _minLength = minLength;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
            if (value is string identityNumber)
            {
                if (identityNumber.Length < _minLength)
                {
                    return new ValidationResult("Identity or Passport number is too short.");
                }

                return ValidationResult.Success!;
            }

            return new ValidationResult("Identity number is required.");
    }
}