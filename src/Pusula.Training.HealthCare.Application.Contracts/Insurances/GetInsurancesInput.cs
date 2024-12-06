using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Insurances
{
    public class GetInsurancesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public string? PolicyNumber { get; set; }
        public decimal? PremiumAmount { get; set; }
        public decimal? CoverageAmount { get; set; }
        public DateTime? StartDateMin { get; set; }
        public DateTime? StartDateMax { get; set; }
        public DateTime? EndDateMin { get; set; }
        public DateTime? EndDateMax { get; set; }
        public EnumInsuranceCompanyName? InsuranceCompanyName { get; set; }
        public string? Description { get; set; }

        public GetInsurancesInput()
        {
        }
    }
}
