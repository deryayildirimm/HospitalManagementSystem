using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class BackgroundCreateInExaminationDto
{
    [StringLength(BackgroundConsts.AllergiesMaxLength)]
    public string? Allergies { get; set; }
    [StringLength(BackgroundConsts.MedicationsMaxLength)]
    public string? Medications { get; set; }
    [StringLength(BackgroundConsts.HabitsMaxLength)]
    public string? Habits { get; set; }
}