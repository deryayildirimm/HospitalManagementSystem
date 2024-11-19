using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentSlotDto
{
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; }

    [Required] public string StartTime { get; set; } = null!;
    
    [Required]
    public string EndTime { get; set; } = null!;
    
    [Required]
    public bool AvailabilityValue { get; set; }
    
}