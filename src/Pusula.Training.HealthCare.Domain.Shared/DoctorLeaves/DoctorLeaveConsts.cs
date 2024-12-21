namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveConsts
{
    private const string DefaultSorting = "{0}StartDate asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DoctorLeave." : string.Empty);
    }
    
    public const int TypeMaxValue = 4;
    public const int TypeMinValue = 0;
    public const int TextMaxLength = 128;
    public const int TextMinLength = 0;
}