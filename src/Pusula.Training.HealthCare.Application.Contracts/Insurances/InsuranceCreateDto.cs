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
        [Required(ErrorMessage = "Please enter a valid Policy Number.")]
        [StringLength(InsuranceConst.MaxPolicyNumberLength, MinimumLength = InsuranceConst.MinPolicyNumberLength,ErrorMessage = "Please enter a valid Policy Number.")]
        public string PolicyNumber { get; set; } = null!;
        public decimal? PremiumAmount { get; set; }
        public decimal? CoverageAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Please choose a Insurance Company")]
        [Range(InsuranceConst.MinInsuranceCompanyName, InsuranceConst.MaxInsuranceCompanyName,ErrorMessage = "Please choose a Insurance Company")]
        public EnumInsuranceCompanyName InsuranceCompanyName { get; set; } = EnumInsuranceCompanyName.None;
        public string? Description { get; set; }
    }
}
