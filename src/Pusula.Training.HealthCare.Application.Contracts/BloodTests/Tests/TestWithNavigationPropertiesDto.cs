using Pusula.Training.HealthCare.BloodTests.Category;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestWithNavigationPropertiesDto
    {
        public TestDto Test { get; set; } = null!;
        public TestCategoryDto TestCategory { get; set; } = null!;
    }
}
