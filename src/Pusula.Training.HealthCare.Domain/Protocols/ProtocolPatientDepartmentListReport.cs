using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolPatientDepartmentListReport
{
    public Guid PatientId { get; set; }
    
    public int  PatientNumber { get; set; }
    
    public string FullName { get; set; }
    
    public string DepartmentName { get; set; }
    
    public int  ProtocolCount { get; set; }
    
    public DateTime? LastVisit { get; set; }
}