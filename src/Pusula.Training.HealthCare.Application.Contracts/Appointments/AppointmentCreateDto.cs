using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentCreateDto
{
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public DateTime AppointmentDate { get; set; }
    
    [Required]
    public DateTime AppointmentTime { get; set; }
    
    [Required]
    public EnumAppointmentStatus Status { get; set; }

    public string? Notes { get; set; } = string.Empty;
    
    [Required]
    public bool ReminderSent { get; set; }
    
    [Required]
    public double Amount { get; set; }

    public AppointmentCreateDto()
    {
    }
}