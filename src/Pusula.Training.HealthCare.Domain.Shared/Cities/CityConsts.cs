namespace Pusula.Training.HealthCare.Cities;

public class CityConsts
{
    private const string DefaultSorting = "{0}Name asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "City." : string.Empty);
    }
    
    public const int NameMaxLength = 128;
    
    public const int NameMinLength = 1;
}