using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeCreateDto
{
    [Required]
    [StringLength(AppointmentTypeConsts.NameMaxLength)]
    public string Name { get; set; } = null!;
}