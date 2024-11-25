
namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResultWithNavigationProperties
    {
        public BloodTestResult BloodTestResult { get; set; } = null!;
        public BloodTest BloodTest { get; set; } = null!;
        public Test Test { get; set; } = null!;
    }
}
