using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Cities;

public class CityCreateDto
{
    
    [Required]
    [StringLength(CityConsts.NameMaxLength, MinimumLength = CityConsts.NameMinLength)]
    public string Name { get; set; } = null!;
}