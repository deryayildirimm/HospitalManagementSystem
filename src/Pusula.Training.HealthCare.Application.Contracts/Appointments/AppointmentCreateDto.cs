using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Validators;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentCreateDto
{
    [Required]
    [GuidValidator]
    public Guid DoctorId { get; set; }
    
    [Required]
    [GuidValidator]
    public Guid PatientId { get; set; }
    
    [Required]
    [GuidValidator]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    [GuidValidator]
    public Guid AppointmentTypeId { get; set; }
    
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