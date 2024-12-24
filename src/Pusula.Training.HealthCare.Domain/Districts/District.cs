using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Districts;

public class District : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public string Name { get; private set; } = null!;

    [NotNull] public Guid CityId { get; private set; }

    protected District()
    {
        Name = string.Empty;
        CityId = Guid.Empty;
    }

    public District(Guid id, Guid cityId, string name)
    {
        Id = id;
        SetName(name);
        SetCityId(cityId);
    }

    public void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DistrictConsts.NameMaxLength, DistrictConsts.NameMinLength);
        Name = name;
    }

    public void SetCityId(Guid cityId)
    {
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        CityId = cityId;
    }
}