using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeManager(IProtocolTypeRepository protocolTypeRepository) : DomainService, IProtocolTypeManager
{
    
    public virtual async Task<ProtocolType> CreateAsync( string? name=null )
    {
        
        Check.NotNullOrWhiteSpace(name, nameof(name), ProtocolTypeConsts.NameMaxLength, ProtocolTypeConsts.NameMinLength);
        
        var protocolType = new ProtocolType(
            GuidGenerator.Create(), name);

        return await protocolTypeRepository.InsertAsync(protocolType);
    
   
    }

    public virtual async Task<ProtocolType> UpdateAsync(Guid id,  string? name=null)
    {
        
        Check.NotNullOrWhiteSpace(name, nameof(name), ProtocolTypeConsts.NameMaxLength, ProtocolTypeConsts.NameMinLength);
        
        var protocolType = await protocolTypeRepository.GetAsync(id);
        
        protocolType.SetName(name);
        
        return await protocolTypeRepository.UpdateAsync(protocolType);
       
    }
    
}