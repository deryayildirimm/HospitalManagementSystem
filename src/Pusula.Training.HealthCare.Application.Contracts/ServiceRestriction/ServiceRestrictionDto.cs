using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.ServiceRestriction;

public class ServiceRestrictionDto
{
    public MedicalServiceDto MedicalService { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public EnumGender Gender { get; set; }
}