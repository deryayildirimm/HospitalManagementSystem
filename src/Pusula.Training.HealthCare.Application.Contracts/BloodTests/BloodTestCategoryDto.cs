using Pusula.Training.HealthCare.BloodTests.Category;
using System;
using System.Text.Json.Serialization;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestCategoryDto
    {
        public Guid BloodTestId { get; set; }
        [JsonIgnore]
        public BloodTestDto BloodTest { get; set; } = null!;

        public Guid TestCategoryId { get; set; }
        public TestCategoryDto TestCategory { get; set; } = null!;
    }
}
