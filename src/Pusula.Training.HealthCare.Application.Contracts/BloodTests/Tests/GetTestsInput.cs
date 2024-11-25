using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class GetTestsInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }
        public string? Name { get; set; } 
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }

        public Guid? TestCategoryId { get; set; }

        public GetTestsInput() { }
    }
}
