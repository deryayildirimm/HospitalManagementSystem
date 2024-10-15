namespace Pusula.Training.HealthCare.Patients
{
    public static class PatientConsts
    {
        private const string DefaultSorting = "{0}FirstName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Patient." : string.Empty);
        }

        public const int FirstNameMaxLength = 128;
        public const int LastNameMaxLength = 128;
        public const int IdentityNumberMaxLength = 11;
        public const int EmailAddressMaxLength = 128;
        public const int MobilePhoneNumberMaxLength = 32;
        public const int GenderMinLength = 0;
        public const int GenderMaxLength = 2;
    }
}