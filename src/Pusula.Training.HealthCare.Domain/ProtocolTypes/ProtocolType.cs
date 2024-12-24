using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolType : AuditedAggregateRoot<Guid>
{
  
    public virtual string Name { get; private set; }

    protected ProtocolType()
    {
        Name = string.Empty;
    }
    
    public ProtocolType(Guid id, string name)
    {
        SetId(id);
        SetName(name);
    }
    
    public void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), ProtocolTypeConsts.NameMaxLength, ProtocolTypeConsts.NameMinLength);
        Name = name;
    }
    
    private void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }
    
    
    
}