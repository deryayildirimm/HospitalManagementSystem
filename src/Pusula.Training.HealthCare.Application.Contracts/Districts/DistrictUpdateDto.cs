using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictUpdateDto
{
    [Required]
    public Guid Id { get; set; } = default!;
    [Required]
    [StringLength(DistrictConsts.NameMaxLength, MinimumLength = DistrictConsts.NameMinLength)]
    public string Name { get; set; } = null!;
    [Required]
    public Guid CityId { get; set; }
}