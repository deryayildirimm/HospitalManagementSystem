using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Titles;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorWithNavigationPropertiesDto
{
    public DoctorDto Doctor { get; set; } = null!;
    public TitleDto Title { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
}