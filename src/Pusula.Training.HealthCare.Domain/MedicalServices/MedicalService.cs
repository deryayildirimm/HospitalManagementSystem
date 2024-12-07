using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalService : FullAuditedAggregateRoot<Guid>
{
    public virtual string Name { get; protected set; }

    public virtual double Cost { get; protected set; }

    public virtual int Duration { get; protected set; }

    public virtual DateTime ServiceCreatedAt { get; protected set; }

    public virtual ICollection<DepartmentMedicalService> DepartmentMedicalServices { get; set; }

    protected MedicalService()
    {
        Name = string.Empty;
        Cost = 0;
        ServiceCreatedAt = DateTime.Now;
        Duration = 0;
        DepartmentMedicalServices = new Collection<DepartmentMedicalService>();
    }

    public MedicalService(Guid id, string name, double cost, int duration, DateTime serviceCreatedAt)
    {
        Id = id;
        SetName(name);
        SetCost(cost);
        SetServiceCreatedAt(serviceCreatedAt);
        SetDuration(duration);
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
            { DepartmentId = departmentId, MedicalServiceId = Id });
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
    
    public void RemoveAllDepartments()
    {
        DepartmentMedicalServices.RemoveAll(x => x.MedicalServiceId == Id);
    }
}