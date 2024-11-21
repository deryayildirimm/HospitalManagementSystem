using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Titles;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorWithNavigationPropertiesDto
{
    public CityDto City { get; set; } = null!;
    public DistrictDto District { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
    public TitleDto Title { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
}