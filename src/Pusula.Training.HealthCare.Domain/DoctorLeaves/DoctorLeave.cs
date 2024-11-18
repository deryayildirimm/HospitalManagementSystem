using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeave : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; set; }
    [NotNull]
    public virtual DateTime StartDate { get; set; }
    [NotNull]
    public virtual DateTime EndDate { get; set; }
    [CanBeNull]
    public virtual string? Reason { get; set; }


    protected DoctorLeave()
    {
        DoctorId = Guid.Empty;
        StartDate = DateTime.Now;
        EndDate = DateTime.Now;
    }

    public DoctorLeave(Guid id, Guid doctorId, DateTime startDate, DateTime endDate, string? reason = null)
    {
        //validasyonlar yapıldı
        Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
        Check.NotNull(startDate, nameof(startDate));
        Check.NotNull(endDate, nameof(endDate));


        Id = id;
        DoctorId = doctorId;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;

    }
    
    
}