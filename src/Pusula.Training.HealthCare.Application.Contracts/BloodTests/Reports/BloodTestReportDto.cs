using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class BloodTestReportDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public Guid BloodTestId { get; set; }
        public BloodTestDto BloodTest { get; set; } = null!;
        public List<BloodTestReportResultDto>? Results { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
