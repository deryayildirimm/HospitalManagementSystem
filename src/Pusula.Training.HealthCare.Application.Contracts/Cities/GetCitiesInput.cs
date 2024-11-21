using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Cities;

public class GetCitiesInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Name { get; set; }
        
    public GetCitiesInput() { }
}