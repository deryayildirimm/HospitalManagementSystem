using System.Collections.Generic;
using Pusula.Training.HealthCare.Departments;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceWithDepartmentsDto
{
    public MedicalServiceDto MedicalService { get; set; } = null!;
    public ICollection<DepartmentDto> Departments { get; set; } = null!;
}