using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class DoctorWorkingHour : FullAuditedEntity<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; protected set; }

    [NotNull]
    public virtual DayOfWeek DayOfWeek { get; protected set; }

    [NotNull]
    public virtual TimeSpan StartHour { get; protected set; }

    [NotNull]
    public virtual TimeSpan EndHour { get; protected set; }
    
    protected DoctorWorkingHour() { }

    public DoctorWorkingHour(Guid doctorId, DayOfWeek dayOfWeek, TimeSpan startHour, TimeSpan endHour)
    {
        SetDoctorId(doctorId);
        SetDayOfWeek(dayOfWeek);
        SetWorkingHours(startHour, endHour);
    }

    private void SetDoctorId(Guid doctorId)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        DoctorId = doctorId;
    }

    private void SetDayOfWeek(DayOfWeek dayOfWeek)
    {
        Check.NotNull(dayOfWeek, nameof(dayOfWeek));
        DayOfWeek = dayOfWeek;
    }

    private void SetWorkingHours(TimeSpan startHour, TimeSpan endHour)
    {
        
        Check.NotNull(startHour, nameof(startHour));
        Check.NotNull(endHour, nameof(endHour));
        
        if (startHour >= endHour)
        {
            throw new ArgumentException("Start hour must be earlier than end hour.");
        }

        if (startHour < DoctorWorkingHoursConsts.MinStartHour || endHour > DoctorWorkingHoursConsts.MaxEndHour)
        {
            throw new ArgumentException($"Working hours must be between {DoctorWorkingHoursConsts.MinStartHour} and {DoctorWorkingHoursConsts.MaxEndHour}.");
        }
        
        StartHour = startHour;
        EndHour = endHour;
    }
    
}