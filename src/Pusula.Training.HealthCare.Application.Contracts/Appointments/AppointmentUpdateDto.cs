using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentUpdateDto
{
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid PatientId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public Guid AppointmentTypeId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public EnumAppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool ReminderSent { get; set; }
    public string? CancellationNotes { get; set; } = string.Empty;
    public double Amount { get; set; }

}