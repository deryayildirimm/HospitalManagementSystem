using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class GetIcdsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? CodeNumber { get; set; }
    public string? Detail { get; set; }
    
    public GetIcdsInput() { }
}