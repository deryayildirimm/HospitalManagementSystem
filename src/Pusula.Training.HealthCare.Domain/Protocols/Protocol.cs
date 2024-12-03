using JetBrains.Annotations;
using System;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Protocols;

public class Protocol : FullAuditedAggregateRoot<Guid>
{
    public virtual string? Notes { get; set; }

    [NotNull]
    public virtual DateTime StartTime { get; set; }

    [CanBeNull]
    public virtual DateTime? EndTime { get; set; }
    public Guid PatientId { get; set; }
    public virtual Patient Patient { get; set; }
    public Guid DepartmentId { get; set; }
    
    public virtual Department Department { get; set; }
    
    public Guid ProtocolTypeId { get; set; }
    
    public virtual ProtocolType ProtocolType { get; set; }
    
    public Guid DoctorId { get; set; }
    
    public virtual Doctor Doctor { get; set; }

    protected Protocol()
    {
        Notes = string.Empty;
        StartTime = DateTime.Now;
    }

    public Protocol(Guid id, Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  DateTime startTime, string? note = null, DateTime? endTime = null) : base(id)
    {
      
        Notes = note;
        StartTime = startTime;
        EndTime = endTime;
        PatientId = patientId;
        DepartmentId = departmentId;
        ProtocolTypeId = protocolTypeId;
        DoctorId = doctorId;
    }

}