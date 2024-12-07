using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Insurances
{
    public class InsuranceCreatedEto : EtoBase
    {
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
