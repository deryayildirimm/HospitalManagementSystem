using System;
using JetBrains.Annotations;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolPatientDoctorListReport
{
    public Guid PatientId { get; set; }
    
    public int  PatientNumber { get; set; }
    
    public string FullName { get; set; }
    
    public string DoctorName { get; set; }
    
    public int  ProtocolCount { get; set; }
    
    public DateTime? LastVisit { get; set; }
}