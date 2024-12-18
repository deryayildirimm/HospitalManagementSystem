using System;

namespace Pusula.Training.HealthCare.Protocols;

public class DepartmentStatisticDto
{
    public Guid DepartmentId { get; set; }
    
    public string DepartmentName { get; set; }
    
    public int  PatientCount { get; set; }
}