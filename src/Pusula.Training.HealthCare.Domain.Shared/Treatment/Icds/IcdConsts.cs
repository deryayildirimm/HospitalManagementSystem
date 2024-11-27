namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdConsts
{
    private const string DefaultSorting = "{0}Code asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Icd." : string.Empty);
    }

    public const int CodeChapterLength = 1;
    
    public const int CodeNumberMaxLength = 6;
    public const int DetailMaxLength = 150;
    
    public const int DetailMinLength = 2;
    public const int CodeNumberMinLength = 2;
}