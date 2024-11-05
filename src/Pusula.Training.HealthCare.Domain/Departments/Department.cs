using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Departments;

public class Department : FullAuditedAggregateRoot<Guid>
{
    [NotNull] 
    public virtual string Name { get; set; }
    
    public virtual ICollection<MedicalService> MedicalServices { get; set; } = new List<MedicalService>();

    protected Department()
    {
        Name = string.Empty;
    }

    public Department(Guid id, string name)
    {
        Id = id;
        Check.NotNull(name, nameof(name));
        Check.Length(name, nameof(name), DepartmentConsts.NameMaxLength, 0);
        Name = name;
    }
}