using System;

namespace Pusula.Training.HealthCare.Protocols;

public class DoctorStatisticDto
{
    public Guid DoctorId { get; set; }
    
    public string? DoctorName { get; set; }
    
    public int  PatientCount { get; set; }
}