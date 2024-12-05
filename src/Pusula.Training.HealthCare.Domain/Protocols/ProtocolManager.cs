using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolManager(IProtocolRepository protocolRepository,
                            IPatientRepository _patientRepository,
                            IDepartmentRepository _departmentRepository,
                            IProtocolTypeRepository _protocolTypeRepository,
                            IDoctorRepository _doctorRepository
    ) : DomainService, IProtocolManager
{
    public virtual async Task<Protocol> CreateAsync(
    Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  DateTime startTime,string? notes = null, DateTime? endTime = null)
    {
        Check.NotNull(protocolTypeId, nameof(protocolTypeId));
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(startTime, nameof(startTime));
        
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE, 
            HealthCareDomainErrorCodes.InvalidDateRange_CODE, 
            startTime > endTime);
        // Repository'den entity'leri getir
        var patient = await _patientRepository.GetAsync(patientId);
        var department = await _departmentRepository.GetAsync(departmentId);
        var protocolType = await _protocolTypeRepository.GetAsync(protocolTypeId);
        var doctor = await _doctorRepository.GetAsync(doctorId);
      
     
        var protocol = new Protocol(
            GuidGenerator.Create(),
            patientId, departmentId, protocolTypeId, doctorId, startTime, doctor, patient, department, protocolType,notes, endTime
        );
  
        Console.WriteLine(patientId);
        Console.WriteLine(protocol);
        
        return await protocolRepository.InsertAsync(protocol);
    }

    public virtual async Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId,Guid protocolTypeId, Guid doctorId, DateTime startTime,string? note = null, DateTime? endTime = null, [CanBeNull] string? concurrencyStamp = null
    )
    {
        
        HealthCareGlobalException.ThrowIf( HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE , 
            HealthCareDomainErrorCodes.InvalidDateRange_CODE, 
            startTime > endTime);

        var protocol = await protocolRepository.GetAsync(id);

        HealthCareGlobalException.ThrowIf(  
            HealthCareDomainErrorCodes.ProtocolUpdate_MESSAGE,
            HealthCareDomainErrorCodes.ProtocolUpdate_CODE,
            protocol == null);

        protocol.SetPatientId(patientId);
        protocol.SetDepartmentId(departmentId);
        protocol.SetProtocolTypeId(protocolTypeId);
        protocol.SetDoctorId(doctorId);
        protocol.SetStartTime(startTime);
        protocol.SetNote(note);
        protocol.SetEndTime(endTime);

        protocol.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await protocolRepository.UpdateAsync(protocol);
    }

}