using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Validators;

public class IdentityNumberValidator : ValidationAttribute
{
    private readonly int _minLength;
    private static readonly Regex _regex = new Regex(@"^[1-9][0-9]{9}[02468]$");

    public IdentityNumberValidator(int minLength = PatientConsts.IdentityNumberLength)
    {
        _minLength = minLength;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var identityFieldProperty = validationContext.ObjectType.GetProperty("identityField");
        var identityFieldValue = identityFieldProperty!.GetValue(validationContext.ObjectInstance);

        if (identityFieldValue is true)
        {
            if (value is string identityNumber)
            {
                if (identityNumber.Length < _minLength)
                {
                    return new ValidationResult("Identity number is too short.");
                }

                if (!_regex.IsMatch(identityNumber))
                {
                    return new ValidationResult("Invalid identity number format.");
                }

                return ValidationResult.Success!;
            }

            return new ValidationResult("Identity number is required.");
        }

        return ValidationResult.Success!;
    }
}