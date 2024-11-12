using System.Collections.Generic;
using Pusula.Training.HealthCare.Departments;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceWithDetails
{
    public MedicalService MedicalService { get; set; } = null!;
    public List<Department> Departments { get; set; } =  new List<Department>();
}