using Pusula.Training.HealthCare.Patients;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Validators
{
    public class PassportNumberValidator : ValidationAttribute
    {
        private readonly int _minLength;

        public PassportNumberValidator(int minLength = PatientConsts.PassportNumberMinLength)
        {
            _minLength = minLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var passportFieldProperty = validationContext.ObjectType.GetProperty("passportField"); //reflection

            var passportFieldValue = passportFieldProperty!.GetValue(validationContext.ObjectInstance);

            if (passportFieldValue is true)
            {
                if (value is string passportNumber)
                {
                    if (passportNumber.Length < _minLength)
                    {
                        return new ValidationResult("Passport number is too short.");
                    }

                    return ValidationResult.Success!;
                }
                return new ValidationResult("Passport number is required.");
            }
            return ValidationResult.Success!;
        }
    }
}