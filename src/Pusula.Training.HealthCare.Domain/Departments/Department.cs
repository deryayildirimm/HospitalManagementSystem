using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Departments;

public class Department : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public virtual string Name { get; protected set; }
    
    public virtual ICollection<Doctor> Doctors { get; protected set; }

    public virtual ICollection<DepartmentMedicalService> DepartmentMedicalServices { get; protected set; }

    protected Department()
    {
        Name = string.Empty;
        DepartmentMedicalServices = new Collection<DepartmentMedicalService>();
        Doctors = new Collection<Doctor>();
    }

    public Department(Guid id, string name)
    {
        Id = id;
        SetName(name);
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(name), DepartmentConsts.NameMaxLength, 0);
        Name = name;
    }
}