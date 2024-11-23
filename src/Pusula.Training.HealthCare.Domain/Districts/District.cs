using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Districts;

public class District : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string Name { get; protected set; }
    
    [NotNull]
    public Guid CityId { get; protected set; }

    protected District()
    {
        Name = string.Empty;
        CityId = Guid.Empty;
    }

    public District(Guid id, Guid cityId, string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.NameMaxLength, DistrictConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        
        Id = id;
        Name = name;
        CityId = cityId;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetCityId(Guid cityId)
    {
        CityId = cityId;
    }
}