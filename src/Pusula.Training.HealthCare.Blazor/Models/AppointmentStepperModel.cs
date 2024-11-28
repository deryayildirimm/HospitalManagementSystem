using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentStepperModel
{
    [Required] 
    public string HospitalName { get; set; } = "XYZ Hospital";

    [Required] 
    public string PatientName { get; set; } = null!;

    [Required] 
    public Guid PatientId { get; set; }

    [Required] 
    public string AppointmentDisplayDate { get; set; } = null!;
        
    [Required] 
    public DateTime AppointmentDate { get; set; }

    [Required] 
    public string AppointmentDisplayTime { get; set; } = null!;
        
    [Required]
    public string StartTime { get; set; } = null!;
    
    [Required]
    public string EndTime { get; set; } = null!;

    [Required] 
    public string DoctorName { get; set; } = null!;

    [Required] 
    public Guid DoctorId { get; set; }
    
    [Required] 
    public Guid AppointmentTypeId { get; set; }

    [Required] 
    public string DepartmentName { get; set; } = null!;

    [Required] 
    public string MedicalServiceName { get; set; } = null!;
        
    public string? Note { get; set; }

    [Required] public bool ReminderSent { get; set; }

    [Required] public Guid MedicalServiceId { get; set; }

    [Required] public double Amount { get; set; } = 0.0;
}