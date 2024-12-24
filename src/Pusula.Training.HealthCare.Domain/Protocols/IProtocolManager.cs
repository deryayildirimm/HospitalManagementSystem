using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Pusula.Training.HealthCare.Protocols;

public interface IProtocolManager
{
    Task<Protocol> CreateAsync(
        string[] medicalServiceNames,
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, Guid insuranceId, DateTime startTime, string? note = null,
        DateTime? endTime = null);
  

     Task<Protocol> UpdateAsync(
        Guid id,
        string[] medicalServiceNames,
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  Guid insuranceId,  DateTime startTime, string? note = null,
        DateTime? endTime = null,  string? concurrencyStamp = null
    );
     


}