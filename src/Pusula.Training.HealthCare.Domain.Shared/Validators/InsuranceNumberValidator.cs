using Pusula.Training.HealthCare.Patients;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pusula.Training.HealthCare.Validators
{
    public class InsuranceNumberValidator : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public InsuranceNumberValidator(int minLength = PatientConsts.InsuranceNumberMinLength, int maxLength = PatientConsts.InsuranceNumberMaxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("The Insurance Number field is required.");
            }

            var insuranceNo = value.ToString();

            if (insuranceNo!.Length < _minLength)
            {
                return new ValidationResult("Insurance number is too short.");
            }

            if (insuranceNo.Length > _maxLength)
            {
                return new ValidationResult("Insurance number is too long.");
            }

            if (!Regex.IsMatch(insuranceNo, "^[A-Za-z0-9]*$"))
            {
                return new ValidationResult("Please enter a valid format.");
            }

            return ValidationResult.Success;
        }
    }
}