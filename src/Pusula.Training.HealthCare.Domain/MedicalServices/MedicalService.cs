using System;
using System.Collections.Generic;
using System.Linq;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalService : FullAuditedAggregateRoot<Guid>
{
    public virtual string Name { get; set; }

    public virtual double Cost { get; set; }
    
    public virtual int Duration { get; set; }

    public virtual DateTime ServiceCreatedAt { get; set; }

    public virtual IList<DepartmentMedicalService> DepartmentMedicalServices { get; set; } =
        new List<DepartmentMedicalService>();

    protected MedicalService()
    {
        Name = string.Empty;
        Cost = 0;
        ServiceCreatedAt = DateTime.Now;
        Duration = 0;
    }

    public MedicalService(Guid id, string name, double cost, int duration, DateTime serviceCreatedAt)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue);
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue, MedicalServiceConsts.DurationMaxValue);
        
        Id = id;
        Name = name;
        Cost = cost;
        ServiceCreatedAt = serviceCreatedAt;
        Duration = duration;
    }
}