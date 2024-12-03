namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeConsts
{
    private const string DefaultSorting = "{0}TitleName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Title." : string.Empty);
    }

    public const int NameMinLength = 1;
    
    public const int NameMaxLength = 30;
}