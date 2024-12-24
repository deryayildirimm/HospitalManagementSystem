using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdCreateDto
{
    [Required(ErrorMessage="Code Number is Required")]
    [StringLength(IcdConsts.CodeNumberMaxLength, MinimumLength = IcdConsts.CodeNumberMinLength)]
    public string CodeNumber { get; set; } = null!;
    [Required(ErrorMessage="Code Number is Required")]
    [StringLength(IcdConsts.DetailMaxLength, MinimumLength = IcdConsts.DetailMinLength)]
    public string Detail { get; set; } = null!;
}