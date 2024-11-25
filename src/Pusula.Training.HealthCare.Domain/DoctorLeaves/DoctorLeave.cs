using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeave : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; protected set; }
    [NotNull]
    public virtual DateTime StartDate { get; protected set; }
    [NotNull]
    public virtual DateTime EndDate { get; protected set; }

    [CanBeNull] public virtual string? Reason { get; set; } = string.Empty;


    protected DoctorLeave()
    {
        DoctorId = Guid.Empty;
        StartDate = DateTime.Now;
        EndDate = DateTime.Now;
    }

    public DoctorLeave(Guid id, Guid doctorId, DateTime startDate, DateTime endDate, string? reason = null)
    {
       
        SetId(id);
        SetDoctorId(doctorId);
        SetStartDate(startDate);
        SetEndDate(endDate);
        SetReason(reason);

    }
    
    public void SetDoctorId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }

    public void SetId(Guid id)
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

    public void SetReason(string? reason)
    {
        Reason = reason;
    }

    
    
}