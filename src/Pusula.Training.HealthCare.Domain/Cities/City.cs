using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Cities;

public class City : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string Name { get; private set; } = null!;

    protected City()
    {
        Name = string.Empty;
    }

    public City(Guid id, string name)
    {
        Id = id;
        SetName(name);
    }

    public void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), CityConsts.NameMaxLength, CityConsts.NameMinLength);
        Name = name;
    }
}