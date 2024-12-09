using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class DoctorWorkingHoursDto : FullAuditedEntityDto<Guid>
{

    public virtual Guid DoctorId { get; protected set; }

    public virtual DayOfWeek DayOfWeek { get; protected set; }

    public virtual TimeSpan StartHour { get; protected set; }

    public virtual TimeSpan EndHour { get; protected set; }

    public DoctorWorkingHoursDto()
    {
        
    }
    
}