namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffConsts
{
    private const string DefaultSorting = "{0}FirstName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "MedicalStaff." : string.Empty);
    }
    
    public const int FirstNameMaxLength = 128;
    public const int LastNameMaxLength = 128;
    public const int IdentityNumberLength = 11;
    public const int GenderMaxValue = 3;
    public const int PhoneNumberMaxLength = 15;
    public const int EmailMaxLength = 128;


    public const int FirstNameMinLength = 1;
    public const int LastNameMinLength = 1;
    public const int GenderMinValue = 0;
    public const int PhoneNumberMinLength = 10;
    public const int EmailMinLength = 5;
}