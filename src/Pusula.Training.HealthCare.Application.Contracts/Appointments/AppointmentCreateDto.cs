using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentCreateDto
{
    [Required]
    public string Name { get; set; } = null!;
}