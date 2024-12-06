using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class GetProtocolTypeInput  : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Name { get; set; }
        
    public GetProtocolTypeInput() { }
}