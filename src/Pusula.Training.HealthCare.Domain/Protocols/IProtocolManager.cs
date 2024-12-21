using System;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Protocols;

public interface IProtocolManager
{
    Task<Protocol> CreateAsync(
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, Guid insuranceId, DateTime startTime, string? note = null,
        DateTime? endTime = null);
  

     Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId,  Guid insuranceId,  DateTime startTime, string? note = null,
        DateTime? endTime = null,  string? concurrencyStamp = null
    );


}