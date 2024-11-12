namespace Pusula.Training.HealthCare.Titles;

public class TitleConsts
{
    private const string DefaultSorting = "{0}TitleName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Title." : string.Empty);
    }

    public const int TitleNameMinLength = 1;
    
    public const int TitleNameMaxLength = 50;
}