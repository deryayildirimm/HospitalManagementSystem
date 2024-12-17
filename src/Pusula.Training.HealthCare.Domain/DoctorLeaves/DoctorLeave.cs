using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeave : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; private set; }

    public virtual Doctor Doctor { get; private set; } = null!;
    
    [NotNull]
    public virtual DateTime StartDate { get; private set; }
    [NotNull]
    public virtual DateTime EndDate { get; private set; }

    [CanBeNull]
    public virtual string? Reason { get; private set; } = string.Empty;

    [NotNull]
    public virtual EnumLeaveType LeaveType { get; private set; }

    protected DoctorLeave()
    {
        DoctorId = Guid.Empty;
        StartDate = DateTime.Now;
        EndDate = DateTime.Now;
        LeaveType = EnumLeaveType.Normal;
    }

    public DoctorLeave(Guid id, Guid doctorId, DateTime startDate, DateTime endDate, EnumLeaveType enumLeaveType, string? reason = null)
    {
       
        SetId(id);
        SetDoctorId(doctorId);
        SetStartDate(startDate);
        SetEndDate(endDate);
        SetReason(reason);
        SetType(enumLeaveType);
    }
    
    public void SetDoctorId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        DoctorId = id;
    }

    private void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }
    
    public void SetStartDate(DateTime startDate)
    {
        Check.NotNull(startDate, nameof(startDate));
        StartDate = startDate;
    }

    public void SetEndDate(DateTime endDate)
    {
        Check.NotNull(endDate, nameof(endDate));
        EndDate = endDate;
    }
    
    public void SetType(EnumLeaveType type)
    {
        Check.NotNull(type, nameof(type));
        LeaveType = type;
    }

    public void SetReason(string? reason)
    {
        Reason = reason;
    }

    
    
}