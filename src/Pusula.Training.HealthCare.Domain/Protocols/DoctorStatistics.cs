using System;

namespace Pusula.Training.HealthCare.Protocols;

public class DoctorStatistics
{
    public Guid DoctorId { get; set; }
    
    public string DoctorName { get; set; }
    
    public int PatientCount { get; set; }
}