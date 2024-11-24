using System.Collections.Generic;
using Pusula.Training.HealthCare.Departments;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceWithDepartments
{
    public MedicalService MedicalService { get; set; } = null!;
    public ICollection<Department> Departments { get; set; } = null!;
}