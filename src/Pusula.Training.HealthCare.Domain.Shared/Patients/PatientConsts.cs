namespace Pusula.Training.HealthCare.Patients
{
    public static class PatientConsts
    {
        private const string DefaultSorting = "{0}FirstName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Patient." : string.Empty);
        }

        public const int NameMaxLength = 128;
        public const int LastNameMaxLength = 128;
        public const int NationalityMaxLength = 210;
        public const int IdentityNumberLength = 11;
        public const int PassportNumberMaxLength = 9;
        public const int GenderMaxValue = 3;
        public const int MobilePhoneNumberMaxLength = 15;
        public const int OnlyPhoneNumberMaxLength = 10;
        public const int EmailAddressMaxLength = 128;
        public const int PatientTypeMaxValue = 3;
        public const int AddressMaxLength = 256;
        public const int InsuranceMaxValue = 2;
        public const int InsuranceNumberMaxLength = 15;
        public const int DiscountGroupMaxValue = 2;
        public const int RelativeMaxValue = 5;
        public const int PhoneCodeMaxLength = 4;

        public const int NationalityMinLength = 0;
        public const int NationalMinLength = 128;
        public const int NameMinLength = 1;
        public const int LastNameMinLength = 1;
        public const int PassportNumberMinLength = 6;
        public const int GenderMinValue = 1;
        public const int MobilePhoneNumberMinLength = 8;
        public const int EmailAddressMinLength = 5;
        public const int PatientTypeMinValue = 1;
        public const int InsuranceMinValue = 1;
        public const int InsuranceNumberMinLength = 8;
        public const int DiscountGroupMinValue = 0;
        public const int RelativeMinValue = 0;
    }
}