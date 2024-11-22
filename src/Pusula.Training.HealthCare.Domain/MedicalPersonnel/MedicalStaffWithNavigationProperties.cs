using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffWithNavigationProperties
{
    public MedicalStaff MedicalStaff { get; set; } = null!;
    public City City { get; set; } = null!;
    public District District { get; set; } = null!;
    public Department Department { get; set; } = null!;
}