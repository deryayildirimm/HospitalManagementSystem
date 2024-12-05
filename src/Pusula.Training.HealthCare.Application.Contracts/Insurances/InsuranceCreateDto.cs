using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Insurances
{
    public class InsuranceCreateDto
    {
        [Required]
        [StringLength(InsuranceConst.MaxPolicyNumberLength, MinimumLength = InsuranceConst.MinPolicyNumberLength)]
        public string PolicyNumber { get; set; } = null!;
        public decimal? PremiumAmount { get; set; }
        public decimal? CoverageAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        [Range(InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName)]
        public EnumInsuranceCompanyName InsuranceCompanyName { get; set; } = EnumInsuranceCompanyName.None;
        public string? Description { get; set; }
    }
}
