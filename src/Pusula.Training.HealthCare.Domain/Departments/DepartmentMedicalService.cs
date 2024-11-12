using System;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Departments;

public class DepartmentMedicalService : Entity
{
    
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
    
    public Guid MedicalServiceId { get; set; }
    public MedicalService MedicalService { get; set; } = null!;
    
    public override object?[] GetKeys()
    {
        return new object?[] { MedicalServiceId, DepartmentId };
    }
}