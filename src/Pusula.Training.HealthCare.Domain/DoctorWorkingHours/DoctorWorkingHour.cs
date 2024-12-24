using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class DoctorWorkingHour : FullAuditedEntity<Guid>
{
    [NotNull] public virtual Guid DoctorId { get; private set; }

    [NotNull] public virtual DayOfWeek DayOfWeek { get; private set; }

    [NotNull] public virtual TimeOnly StartHour { get; private set; }

    [NotNull] public virtual TimeOnly EndHour { get; private set; }

    protected DoctorWorkingHour()
    {
    }

    public DoctorWorkingHour(Guid doctorId, DayOfWeek dayOfWeek, TimeOnly startHour, TimeOnly endHour)
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

    private void SetWorkingHours(TimeOnly startHour, TimeOnly endHour)
    {
        Check.NotNull(startHour, nameof(startHour));
        Check.NotNull(endHour, nameof(endHour));

        StartHour = startHour;
        EndHour = endHour;
    }
}