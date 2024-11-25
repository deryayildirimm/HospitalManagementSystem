using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Cities;

public class CityUpdateDto
{
    [Required] 
    [StringLength(100)] 
    public Guid Id { get; set; } = default!;
    
    [Required]
    [StringLength(CityConsts.NameMaxLength, MinimumLength = CityConsts.NameMinLength)]
    public string Name { get; set; } = null!;
}