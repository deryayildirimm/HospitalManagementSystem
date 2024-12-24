using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeExcelDownloadDto : PagedAndSortedResultRequestDto
{
    
    public string DownloadToken { get; set; } = null!;
    
    public string? FilterText { get; set; }
    
    public string Name { get; set; } = null!;
    

    public ProtocolTypeExcelDownloadDto()
    {
        
    }
    
}