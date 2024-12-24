using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
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
    
    public virtual DateTime? EndTime { get; private set; }
    public Guid PatientId { get; private set; }
    public virtual Patient Patient { get;  set; } = null!; 
    public Guid DepartmentId { get; private set; }
    
    public virtual Department Department { get;  set; } = null!; 
    
    public Guid ProtocolTypeId { get; private set; }
    
    public virtual ProtocolType ProtocolType { get; set; } = null!; 
    
    public Guid DoctorId { get; private set; }
    
    public virtual Doctor Doctor { get; init; } = null!; 
    
    public Guid InsuranceId { get; private set; }

    public virtual Insurance Insurance { get; init; } = null!; 
    
    public IList<ProtocolMedicalService> ProtocolMedicalServices { get;  private set; } 
       


    protected Protocol()
    {
        Note = string.Empty;
        StartTime = DateTime.Now.Date;
        ProtocolMedicalServices = new List<ProtocolMedicalService>();
    }

    public Protocol(Guid id, Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, Guid insuranceId,  DateTime startTime, string? note = null, DateTime? endTime = null ) : base(id)
    {
      
        SetId(id);
        SetPatientId(patientId);
        SetDepartmentId(departmentId);
        SetDoctorId(doctorId);
        SetStartTime(startTime);
        SetEndTime(endTime);
        SetProtocolTypeId(protocolTypeId);
        SetInsuranceId(insuranceId);
        SetNote(note);
        ProtocolMedicalServices = new List<ProtocolMedicalService>();
    }
    
    // add medical service 
    public void AddMedicalService(Guid medicalServiceId)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));

        if (IsInMedicalservice(medicalServiceId))
        {
            return;
        }
        ProtocolMedicalServices.Add(new ProtocolMedicalService(protocolId : Id, medicalServiceId));
    }

    //remove medical service 
    public void RemoveMedicalService(Guid medicalServiceId)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));

        if (!IsInMedicalservice(medicalServiceId))
        {
            return;
        }

        ProtocolMedicalServices.RemoveAll(x => x.MedicalServiceId == medicalServiceId);
    }

    public void RemoveAllMedicalServicesExceptGivenIds(List<Guid> medicalServiceIds)
    {
        Check.NotNullOrEmpty(medicalServiceIds, nameof(medicalServiceIds));
            
        ProtocolMedicalServices.RemoveAll(x => !medicalServiceIds.Contains(x.MedicalServiceId));
    }

    public void RemoveAllMedicalServices()
    {
        ProtocolMedicalServices.RemoveAll(x => x.ProtocolId == Id);
    }

    private bool IsInMedicalservice(Guid medicalServiceId)
    {
        return ProtocolMedicalServices.Any(x => x.MedicalServiceId == medicalServiceId);
    }
    
    private void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }

    public void SetStartTime(DateTime startTime)
    {
        Check.NotNull(startTime, nameof(startTime));
        StartTime = startTime;
    }

    public void SetEndTime(DateTime? endTime) =>   EndTime = endTime;
 
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
    
     public void SetInsuranceId(Guid insuranceId)
     {
            Check.NotNull(insuranceId, nameof(insuranceId));
            InsuranceId = insuranceId;
     }
    public void SetProtocolTypeId(Guid protocolTypeId)
    {
        Check.NotNull(protocolTypeId, nameof(protocolTypeId));
        ProtocolTypeId = protocolTypeId;
    }

    public void SetNote(string? note) => Note = note;
    


}