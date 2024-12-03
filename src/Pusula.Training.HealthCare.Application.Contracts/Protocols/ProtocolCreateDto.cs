using System;
using System.ComponentModel.DataAnnotations;

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
}