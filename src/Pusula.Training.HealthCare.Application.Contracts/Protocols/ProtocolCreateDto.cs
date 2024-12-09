using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolCreateDto
{

    public string? Notes { get; set; } = null!;
    [Required] public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid PatientId { get; set; }
    public Guid DepartmentId { get; set; }
    
    public Guid DoctorId { get; set; }
    
    public Guid ProtocolTypeId { get; set; }
    
    public Guid InsuranceId { get; set; }
   

}