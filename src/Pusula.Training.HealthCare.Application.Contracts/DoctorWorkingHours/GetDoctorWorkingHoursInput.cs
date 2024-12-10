using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class GetDoctorWorkingHoursInput : PagedAndSortedResultRequestDto
{
    public virtual Guid? DoctorId { get; protected set; } = null;

    public GetDoctorWorkingHoursInput()
    {
        
    }
}