using System;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public interface IProtocolTypeManager
{
    Task<ProtocolType> CreateAsync(string? name=null);

    Task<ProtocolType> UpdateAsync(Guid id,  string? name=null);
}