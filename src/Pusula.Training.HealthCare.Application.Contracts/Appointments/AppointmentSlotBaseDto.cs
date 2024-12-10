using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentSlotBaseDto
{
    [Required]
    public DateTime Date { get; set; }

    [Required] 
    public string StartTime { get; set; } = null!;
    
    [Required]
    public string EndTime { get; set; } = null!;
}