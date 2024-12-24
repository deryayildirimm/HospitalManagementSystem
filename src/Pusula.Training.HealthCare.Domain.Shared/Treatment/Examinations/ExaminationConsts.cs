namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationConsts
{
    private const string DefaultSorting = "{0}Complaint asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Examination." : string.Empty);
    }
    
    public const int ComplaintMaxLength = 128;
    public const int StoryMaxLength = 128;
    
    public const int ComplaintMinLength = 2;

}