using System;
using System.Collections.Generic;
using System.Linq;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Protocols;
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
    
    public virtual IList<ProtocolMedicalService> ProtocolMedicalServices { get; set; } =
        new List<ProtocolMedicalService>();

    protected MedicalService()
    {
        Name = string.Empty;
        Cost = 0;
        ServiceCreatedAt = DateTime.Now;
        Duration = 0;
    }

    public MedicalService(Guid id, string name, double cost, int duration, DateTime serviceCreatedAt)
    {
        SetId(id);
        SetName(name);
        SetCost(cost);
        SetServiceCreatedAt(serviceCreatedAt);
        SetDuration(duration);
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

    public void SetCost(double cost)
    {
        Check.Range(cost, nameof(cost), MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue);
        Cost = cost;
    }

    public void SetServiceCreatedAt(DateTime serviceCreatedAt)
    {
        Check.NotNull(serviceCreatedAt, nameof(serviceCreatedAt));
        ServiceCreatedAt = serviceCreatedAt;
    }

    public void SetDuration(int duration)
    {
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue, MedicalServiceConsts.DurationMaxValue);
        Duration = duration;
    }

}