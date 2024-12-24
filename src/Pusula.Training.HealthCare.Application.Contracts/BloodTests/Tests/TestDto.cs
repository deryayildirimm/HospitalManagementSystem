using Pusula.Training.HealthCare.BloodTests.Category;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public virtual string Name { get; set; } = null!;
        [Required]
        public virtual double MinValue { get; set; }
        [Required]
        public virtual double MaxValue { get; set; }
        [Required]
        public virtual Guid TestCategoryId { get; set; }
        public virtual TestCategoryDto TestCategory { get; set; } = null!;

        public string ConcurrencyStamp { get; set; } = null!;

    }
}
