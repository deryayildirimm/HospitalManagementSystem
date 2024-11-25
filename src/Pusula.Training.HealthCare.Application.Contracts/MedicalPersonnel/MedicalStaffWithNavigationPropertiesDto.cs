using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffWithNavigationPropertiesDto
{
    public MedicalStaffDto MedicalStaff { get; set; } = null!;
    public CityDto City { get; set; } = null!;
    public DistrictDto District { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
}