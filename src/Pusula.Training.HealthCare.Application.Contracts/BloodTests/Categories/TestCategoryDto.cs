using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Category
{
    public class TestCategoryDto : FullAuditedEntityDto<Guid>
    {
        [Required]
        [StringLength(BloodTestConst.CategoryNameMax,MinimumLength = BloodTestConst.CategoryNameMin)]
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
