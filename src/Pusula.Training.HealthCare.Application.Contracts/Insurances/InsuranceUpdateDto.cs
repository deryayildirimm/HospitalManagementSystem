using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Insurances
{
    public class InsuranceUpdateDto : IHasConcurrencyStamp
    {
        [StringLength(InsuranceConst.MaxPolicyNumberLength, MinimumLength = InsuranceConst.MinPolicyNumberLength, ErrorMessage = "Please enter a valid Policy Number")]
        public string PolicyNumber { get; set; } = null!;
        public decimal? PremiumAmount { get; set; }
        public decimal? CoverageAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Range(InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName, ErrorMessage ="Please select a Insurance Company")]
        public EnumInsuranceCompanyName InsuranceCompanyName { get; set; } = EnumInsuranceCompanyName.None;
        public string? Description { get; set; }
        public string ConcurrencyStamp { get; set; } = null!;

    }
}
