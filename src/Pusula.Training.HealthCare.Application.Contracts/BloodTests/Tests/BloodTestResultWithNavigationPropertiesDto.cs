
namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResultWithNavigationPropertiesDto
    {
        public BloodTestResultDto BloodTestResult { get; set; } = null!;
        public BloodTestDto BloodTest{ get; set; } = null!;
        public TestDto Test { get; set; } = null!;
    }
}
