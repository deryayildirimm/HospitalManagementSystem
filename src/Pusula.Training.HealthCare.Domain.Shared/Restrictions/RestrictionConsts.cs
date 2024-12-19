namespace Pusula.Training.HealthCare.Restrictions;

public class RestrictionConsts
{
    private const string DefaultSorting = "{0}DepartmentId asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Restriction." : string.Empty);
    }
    
    public const int GenderMinValue = 0;
    public const int GenderMaxValue = 3;
    
    public const int AgeMinValue = 0;
    public const int AgeMaxValue = 250;
}