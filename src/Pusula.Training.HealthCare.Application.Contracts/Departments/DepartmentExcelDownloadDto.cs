using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Departments;

public class DepartmentExcelDownloadDto : PagedAndSortedResultRequestDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public DepartmentExcelDownloadDto()
    {
    }
}