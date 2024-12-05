using JetBrains.Annotations;
using System;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Protocols;

public class Protocol : FullAuditedAggregateRoot<Guid>
{
    public virtual string? Note { get; private set; }

    [NotNull]
    public virtual DateTime StartTime { get; private set; }

    [CanBeNull]
    public virtual DateTime? EndTime { get; private set; }
    public Guid PatientId { get; private set; }
    public virtual Patient Patient { get;  set; }
    public Guid DepartmentId { get; private set; }
    
    public virtual Department Department { get;  set; }
    
    public Guid ProtocolTypeId { get; private set; }
    
    public virtual ProtocolType ProtocolType { get; set; }
    
    public Guid DoctorId { get; private set; }
    
    public virtual Doctor Doctor { get; set; }

    protected Protocol()
    {
        Note = string.Empty;
        StartTime = DateTime.Now;
    }

    public Protocol(Guid id, Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  DateTime startTime, string? note = null, DateTime? endTime = null ) : base(id)
    {
      
        SetId(id);
        SetPatientId(patientId);
        SetDepartmentId(departmentId);
        SetDoctorId(doctorId);
        SetStartTime(startTime);
        SetEndTime(endTime);
        SetProtocolTypeId(protocolTypeId);
        SetNote(note);

  

    }
    
    public void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }

    public void SetStartTime(DateTime startTime)
    {
        Check.NotNull(startTime, nameof(startTime));
        StartTime = startTime;
    }

    public void SetEndTime(DateTime? endTime)
    {
       
        EndTime = endTime;
    }
    
    public void SetDoctorId(Guid doctorId)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        DoctorId = doctorId;
    }
    

    public void SetPatientId(Guid patientId)
    {
        Check.NotNull(patientId, nameof(patientId));
        PatientId = patientId;
    }

    public void SetDepartmentId(Guid departmentId)
    {
        Check.NotNull(departmentId, nameof(departmentId));
        DepartmentId = departmentId;
    }

    public void SetProtocolTypeId(Guid protocolTypeId)
    {
        Check.NotNull(protocolTypeId, nameof(protocolTypeId));
        ProtocolTypeId = protocolTypeId;
    }
 
    public void SetNote(string? note)
    {
        
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidNoteLength_MESSAGE, 
            HealthCareDomainErrorCodes.InvalidNoteLength_CODE, 
            !string.IsNullOrWhiteSpace(note) || (note?.Length > ProtocolConsts.MaxNotesLength || ProtocolConsts.MinNotesLength < note?.Length ));
        
        Note = note;
    }   

    

}