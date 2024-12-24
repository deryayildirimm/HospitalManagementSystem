using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public Guid Id { get; set; } = default!;
        [Required]
        public virtual string Name { get; set; } = null!;
        [Required]
        public virtual double MinValue { get; set; }
        [Required]
        public virtual double MaxValue { get; set; }
        [Required]
        public virtual Guid TestCategoryId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
