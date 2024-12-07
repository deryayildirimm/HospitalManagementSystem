namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeConsts
{
    private const string DefaultSorting = "{0}Name asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "ProtocolType." : string.Empty);
    }

    public const int NameMinLength = 1;
    
    public const int NameMaxLength = 30;
}