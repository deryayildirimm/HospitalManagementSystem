using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Category
{
    public class GetTestCategoriesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; } 
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public double? Price { get; set; }

        public GetTestCategoriesInput() { }
    }
}
