using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Cities;

public class City : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string Name { get; protected set; }

    protected City()
    {
        Name = string.Empty;
    }

    public City(Guid id, string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), CityConsts.NameMaxLength, CityConsts.NameMinLength);

        Id = id;
        Name = name;
    }

    public void SetName(string name)
    {
        Name = name;
    }
}