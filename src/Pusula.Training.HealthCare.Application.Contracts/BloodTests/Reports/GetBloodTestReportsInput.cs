using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Reports
{
    public class GetBloodTestReportsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public Guid? BloodTestId { get; set; }
        public GetBloodTestReportsInput(){ }
    }
}
