using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResultDto : AuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public virtual double Value { get; set; }
        [Required]
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        public virtual Guid BloodTestId { get; set; }
        public virtual Guid TestId { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;
    }
}
