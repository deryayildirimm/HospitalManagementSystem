using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolManager(IProtocolRepository protocolRepository) : DomainService, IProtocolManager
{
    public virtual async Task<Protocol> CreateAsync(
    Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  DateTime startTime,string? note = null, DateTime? endTime = null)
    {
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNullOrWhiteSpace(note, nameof(note));
        Check.Length(note, nameof(note), ProtocolConsts.TypeMaxLength, ProtocolConsts.TypeMinLength);
        Check.NotNull(startTime, nameof(startTime));

        var protocol = new Protocol(
         GuidGenerator.Create(),
         patientId, departmentId, protocolTypeId,doctorId,  startTime, note, endTime
         );

        return await protocolRepository.InsertAsync(protocol);
    }

    public virtual async Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId,Guid protocolTypeId, Guid doctorId, DateTime startTime,string? note = null, DateTime? endTime = null, [CanBeNull] string? concurrencyStamp = null
    )
    {
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNullOrWhiteSpace(note, nameof(note));
        Check.Length(note, nameof(note), ProtocolConsts.TypeMaxLength, ProtocolConsts.TypeMinLength);
        Check.NotNull(startTime, nameof(startTime));

        var protocol = await protocolRepository.GetAsync(id);

        protocol.PatientId = patientId;
        protocol.DepartmentId = departmentId;
        protocol.Notes = note;
        protocol.StartTime = startTime;
        protocol.EndTime = endTime;
        protocol.ProtocolTypeId = protocolTypeId;
        protocol.DoctorId = doctorId;

        protocol.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await protocolRepository.UpdateAsync(protocol);
    }

}