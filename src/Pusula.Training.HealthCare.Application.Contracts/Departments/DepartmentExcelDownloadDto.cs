namespace Pusula.Training.HealthCare.Departments;

public class DepartmentExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public DepartmentExcelDownloadDto()
    {
    }
}