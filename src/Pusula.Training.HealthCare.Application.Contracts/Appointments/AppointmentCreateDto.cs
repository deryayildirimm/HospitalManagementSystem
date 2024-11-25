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
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [StringLength(AppointmentConsts.MaxNotesLength)]
    public string? Notes { get; set; } = string.Empty;
    
    [Required]
    public bool ReminderSent { get; set; }
    
    [Required]
    [Range(AppointmentConsts.MinAmount, AppointmentConsts.MaxAmount)]
    public double Amount { get; set; }
}