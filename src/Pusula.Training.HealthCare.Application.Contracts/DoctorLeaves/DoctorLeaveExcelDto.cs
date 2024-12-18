using System;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveExcelDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public virtual EnumLeaveType LeaveType { get; set; }
    public virtual string? Reason { get; set; }
}