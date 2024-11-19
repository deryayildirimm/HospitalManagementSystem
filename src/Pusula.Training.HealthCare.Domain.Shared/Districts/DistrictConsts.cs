namespace Pusula.Training.HealthCare.Districts;

public class DistrictConsts
{
    private const string DefaultSorting = "{0}Name asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "District." : string.Empty);
    }
    
    public const int NameMaxLength = 128;
    
    public const int NameMinLength = 1;
}