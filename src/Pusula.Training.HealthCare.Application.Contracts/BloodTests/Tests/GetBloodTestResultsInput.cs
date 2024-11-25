using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class GetBloodTestResultsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public double? Value { get; set; }
        public BloodResultStatus? BloodResultStatus { get; set; }
        public Guid? BloodTestId { get; set; }
        public Guid? TestId { get; set; }
        public GetBloodTestResultsInput() { }
    }
}
