using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public class TestCategoryCreateDto
    {
        [Required]
        [StringLength(BloodTestConst.CategoryNameMax, MinimumLength = BloodTestConst.CategoryNameMin)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(BloodTestConst.CategoryDescriptionMax, MinimumLength = BloodTestConst.CategoryDescriptionMin)]
        public string Description { get; set; } = null!;

        [Required]
        public string Url { get; set; } = null!;

        [Required]
        public double Price { get; set; } 
    }
}
