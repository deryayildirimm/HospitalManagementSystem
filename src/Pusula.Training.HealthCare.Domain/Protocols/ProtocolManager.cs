
using System;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolManager(
    IProtocolRepository protocolRepository, 
    IMedicalServiceRepository medicalServiceRepository) : DomainService, IProtocolManager
{
    

    
    public virtual async Task<Protocol> CreateAsync(
         string[]? medicalServiceNames,
    Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, Guid insuranceId,  DateTime startTime,string? notes = null, DateTime? endTime = null)
    {
     
        Check.NotNull(protocolTypeId, nameof(protocolTypeId) );
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(insuranceId, nameof(insuranceId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(startTime, nameof(startTime));
        
            
        HealthCareGlobalException.ThrowIf(" Department needed!!", 
            departmentId == Guid.Empty);
        
        HealthCareGlobalException.ThrowIf(" Doctor needed!!", 
            doctorId == Guid.Empty);
        
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
         string[]? medicalServiceNames,
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

        await SetMedicalAsync(protocol, medicalServiceNames);
        
        return await protocolRepository.UpdateAsync(protocol);
    }
    
    
    private async Task SetMedicalAsync(Protocol protocol, string[]? medicalServiceNames)
    {
        if (medicalServiceNames == null )
        {
            protocol.RemoveAllMedicalServices();
            return;
        }

        // medical servise de search yapıyor
        // ismi girilen tıbbi hizmeti arıyor tabloda 
        // ve o id yi alıyor
        var query = (await medicalServiceRepository.GetQueryableAsync())
            .Where(x => medicalServiceNames.Contains(x.Name))
            .Select(x => x.Id)
            .Distinct();

        
            // bu id lerden liste oluşturuyor 
            // hiç yoksa dönüyor zaten çıkıyor metoddan 
        var medicalServiceIds = await AsyncExecuter.ToListAsync(query);
        if (!medicalServiceIds.Any())
        {
            return;
        }

        protocol.RemoveAllMedicalServicesExceptGivenIds(medicalServiceIds);

        foreach (var protocolId in medicalServiceIds)
        {
            protocol.AddMedicalService(protocolId);
        }
    }
    
    

}