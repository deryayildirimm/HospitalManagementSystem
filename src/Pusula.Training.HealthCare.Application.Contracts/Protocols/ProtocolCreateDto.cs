using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolCreateDto
{

    public string? Notes { get; set; } = null!;
    [Required] 
    public DateTime StartTime { get; set; } = DateTime.Today;
    public DateTime? EndTime { get; set; }
    
    [Required(ErrorMessage = "Patient is required.")]
    public Guid PatientId { get; set; }
    [Required(ErrorMessage = "Department is required.")]
    public Guid DepartmentId { get; set; }
    
    [Required(ErrorMessage = "Doctor is required.")] 
    public Guid DoctorId { get; set; }
    [Required(ErrorMessage = "ProtocolType is required.")]
    public Guid ProtocolTypeId { get; set; }
    
    [Required(ErrorMessage = "Insurance is required.")] 
    public Guid InsuranceId { get; set; }

    public string[]? MedicalServiceNames;



}