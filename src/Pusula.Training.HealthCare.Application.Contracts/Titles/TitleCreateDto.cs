using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Titles;

public class TitleCreateDto
{
    [Required]
    [StringLength(TitleConsts.TitleNameMaxLength)]
    public string TitleName { get; set; } = null!;
}