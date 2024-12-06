using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Insurances
{
    public class InsuranceDto : FullAuditedEntityDto<Guid> , IHasConcurrencyStamp
    {
        public string PolicyNumber { get; set; } = null!;
        public decimal? PremiumAmount { get; set; }
        public decimal? CoverageAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EnumInsuranceCompanyName InsuranceCompanyName { get; set; } = EnumInsuranceCompanyName.None;
        public string? Description { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

    }
}
