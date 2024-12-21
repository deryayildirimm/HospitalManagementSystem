
using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolManager(IProtocolRepository protocolRepository
    ) : DomainService, IProtocolManager
{
    public virtual async Task<Protocol> CreateAsync(
    Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, Guid insuranceId,  DateTime startTime,string? notes = null, DateTime? endTime = null)
    {
     
        Check.NotNull(protocolTypeId, nameof(protocolTypeId));
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(insuranceId, nameof(insuranceId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(startTime, nameof(startTime));
        
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDateRange_MESSAGE, 
            HealthCareDomainErrorCodes.InvalidDateRange_CODE, 
            startTime > endTime);
        
        var protocol = new Protocol(
            GuidGenerator.Create(),
            patientId, departmentId, protocolTypeId, doctorId, insuranceId, startTime,notes, endTime
        );
        
        return await protocolRepository.InsertAsync(protocol);
    }

    public virtual async Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId,Guid protocolTypeId, Guid doctorId, Guid insuranceId, DateTime startTime,string? note = null, DateTime? endTime = null, string? concurrencyStamp = null
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

        protocol!.SetPatientId(patientId);
        protocol.SetDepartmentId(departmentId);
        protocol.SetProtocolTypeId(protocolTypeId);
        protocol.SetDoctorId(doctorId);
        protocol.SetInsuranceId(insuranceId);
        protocol.SetStartTime(startTime);
        protocol.SetNote(note);
        protocol.SetEndTime(endTime);

        protocol.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await protocolRepository.UpdateAsync(protocol);
    }

}