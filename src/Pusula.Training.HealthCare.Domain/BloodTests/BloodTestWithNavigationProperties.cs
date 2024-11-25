using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestWithNavigationProperties
    {
        public BloodTest BloodTest { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public TestCategory TestCategory { get; set; } = null!;
    }
}
