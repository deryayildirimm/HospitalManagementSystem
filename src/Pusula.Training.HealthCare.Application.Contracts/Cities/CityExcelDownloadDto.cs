namespace Pusula.Training.HealthCare.Cities;

public class CityExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }
    public string? Name { get; set; }
}