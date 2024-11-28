using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentType : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public virtual string Name { get; set; }

    protected AppointmentType()
    {
        Name = string.Empty;
    }

    public AppointmentType(Guid id, string name)
    {
        SetId(id);
        SetName(name);
    }
    
    private void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }

    public void SetName(string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Name = name;
    }
}