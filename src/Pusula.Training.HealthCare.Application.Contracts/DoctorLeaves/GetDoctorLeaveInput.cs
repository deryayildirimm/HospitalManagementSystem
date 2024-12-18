using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class GetDoctorLeaveInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EnumLeaveType? LeaveType { get; set; }
    public string? Reason { get; set; }

    public GetDoctorLeaveInput()
    {
    }
}