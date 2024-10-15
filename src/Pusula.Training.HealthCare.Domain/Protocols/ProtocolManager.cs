using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolManager(IProtocolRepository protocolRepository) : DomainService
{
    public virtual async Task<Protocol> CreateAsync(
    Guid patientId, Guid departmentId, string type, DateTime startTime, string? endTime = null)
    {
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), ProtocolConsts.TypeMaxLength, ProtocolConsts.TypeMinLength);
        Check.NotNull(startTime, nameof(startTime));

        var protocol = new Protocol(
         GuidGenerator.Create(),
         patientId, departmentId, type, startTime, endTime
         );

        return await protocolRepository.InsertAsync(protocol);
    }

    public virtual async Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId, string type, DateTime startTime, string? endTime = null, [CanBeNull] string? concurrencyStamp = null
    )
    {
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNullOrWhiteSpace(type, nameof(type));
        Check.Length(type, nameof(type), ProtocolConsts.TypeMaxLength, ProtocolConsts.TypeMinLength);
        Check.NotNull(startTime, nameof(startTime));

        var protocol = await protocolRepository.GetAsync(id);

        protocol.PatientId = patientId;
        protocol.DepartmentId = departmentId;
        protocol.Type = type;
        protocol.StartTime = startTime;
        protocol.EndTime = endTime;

        protocol.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await protocolRepository.UpdateAsync(protocol);
    }

}