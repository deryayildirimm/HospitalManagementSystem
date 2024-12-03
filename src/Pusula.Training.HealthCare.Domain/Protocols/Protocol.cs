using JetBrains.Annotations;
using System;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Protocols;

public class Protocol : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string Type { get; set; }

    public virtual DateTime StartTime { get; set; }

    [CanBeNull]
    public virtual string? EndTime { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DepartmentId { get; set; }
    
    public virtual Department Department { get; set; }
    
    public Guid ProtocolTypeId { get; set; }
    
    public virtual ProtocolType ProtocolType { get; set; }

    protected Protocol()
    {
        Type = string.Empty;
    }

    public Protocol(Guid id, Guid patientId, Guid departmentId, Guid protocolTypeId, string type, DateTime startTime, string? endTime = null) : base(id)
    {
        Check.NotNull(type, nameof(type));
        Check.Length(type, nameof(type), ProtocolConsts.TypeMaxLength, ProtocolConsts.TypeMinLength);
        Type = type;
        StartTime = startTime;
        EndTime = endTime;
        PatientId = patientId;
        DepartmentId = departmentId;
        ProtocolTypeId = protocolTypeId;
    }

}