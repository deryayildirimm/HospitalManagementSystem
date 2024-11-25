using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestWithNavigationPropertiesDto
    {
        public BloodTestDto BloodTest { get; set; } = null!;
        public DoctorDto Doctor { get; set; } = null!;
        public PatientDto Patient { get; set; } = null!;
        public TestCategoryDto TestCategory { get; set; } = null!;
    }
}
