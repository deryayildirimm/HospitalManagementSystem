using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestCreateDto
    {
        [Required]
        public virtual string Name { get; set; } = null!;
        [Required]
        public virtual double MinValue { get; set; } 
        [Required]
        public virtual double MaxValue { get; set; }
        [Required]
        public virtual Guid TestCategoryId { get; set; }
    }
}
