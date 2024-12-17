namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveConsts
{
    private const string DefaultSorting = "{0}StartDate asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DoctorLeave." : string.Empty);
    }
    
}