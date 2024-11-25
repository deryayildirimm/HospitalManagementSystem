using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Titles;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorWithNavigationProperties
{
    public Doctor Doctor { get; set; } = null!;
    public City City { get; set; } = null!;
    public District District { get; set; } = null!;
    public Title Title { get; set; } = null!;
    public Department Department { get; set; } = null!;
}