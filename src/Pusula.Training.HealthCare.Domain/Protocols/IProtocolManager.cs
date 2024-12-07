using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Pusula.Training.HealthCare.Protocols;

public interface IProtocolManager
{
    Task<Protocol> CreateAsync(
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, DateTime startTime, string? note = null,
        DateTime? endTime = null);
  

     Task<Protocol> UpdateAsync(
        Guid id,
        Guid patientId, Guid departmentId, Guid protocolTypeId, Guid doctorId, DateTime startTime, string? note = null,
        DateTime? endTime = null, [CanBeNull] string? concurrencyStamp = null
    );


}