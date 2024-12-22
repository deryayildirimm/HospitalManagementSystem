using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Protocols;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalService : FullAuditedAggregateRoot<Guid>
{
    [NotNull] public virtual string Name { get; private set; } = null!;

    [NotNull] public virtual double Cost { get; private set; }

    [NotNull] public virtual int Duration { get; private set; }

    [NotNull] public virtual DateTime ServiceCreatedAt { get; private set; }

    public virtual ICollection<DepartmentMedicalService> DepartmentMedicalServices { get; private set; } = null!;

    public virtual ICollection<ProtocolMedicalService> ProtocolMedicalServices { get; private set; } = null!;

    protected MedicalService()
    {
        Name = string.Empty;
        Cost = 0;
        ServiceCreatedAt = DateTime.Now;
        Duration = 0;
        DepartmentMedicalServices = new List<DepartmentMedicalService>();
        ProtocolMedicalServices = new List<ProtocolMedicalService>();
    }

    public MedicalService(Guid id, string name, double cost, int duration, DateTime serviceCreatedAt)
    {
        Id = id;
        SetName(name);
        SetCost(cost);
        SetServiceCreatedAt(serviceCreatedAt);
        SetDuration(duration);
        DepartmentMedicalServices = new List<DepartmentMedicalService>();
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
        Check.Range(duration, nameof(duration), MedicalServiceConsts.DurationMinValue,
            MedicalServiceConsts.DurationMaxValue);
        Duration = duration;
    }

    public void AddDepartment(Guid departmentId)
    {
        if (IsInDepartment(departmentId))
        {
            return;
        }

        DepartmentMedicalServices.Add(new DepartmentMedicalService
        {
            DepartmentId = departmentId,
            MedicalServiceId = Id
        });
    }

    private bool IsInDepartment(Guid departmentId)
    {
        return DepartmentMedicalServices.Any(x => x.DepartmentId == departmentId);
    }

    public void RemoveAllDepartmentsExceptGivenIds(List<Guid> departmentIds)
    {
        Check.NotNullOrEmpty(departmentIds, nameof(departmentIds));
        DepartmentMedicalServices.RemoveAll(x => !departmentIds.Contains(x.DepartmentId));
    }

    public void RemoveAll()
    {
        DepartmentMedicalServices.RemoveAll(x => x.MedicalServiceId == Id);
    }
}