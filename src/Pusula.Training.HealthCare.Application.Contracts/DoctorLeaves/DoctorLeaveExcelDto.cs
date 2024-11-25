using System;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveExcelDto
{
 
    public Guid DoctorId { get; set; } = Guid.Empty;
   
    public virtual DateTime StartDate { get; set; } = DateTime.Today;
    
    public virtual DateTime EndDate { get; set; } = DateTime.Today;
   
    public virtual string? Reason { get; set; }
}