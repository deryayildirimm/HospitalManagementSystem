using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.BloodTests.Tests;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class BloodTestResultDto : AuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public virtual double Value { get; set; }
        [Required]
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        [Required]
        public virtual Guid TestId { get; set; }
        public virtual TestDto Test { get; set; } = null!;
        public string ConcurrencyStamp { get; set; } = null!;
    }
}
