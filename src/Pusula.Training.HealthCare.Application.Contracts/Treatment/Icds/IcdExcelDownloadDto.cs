namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }
    public string? CodeNumber { get; set; }
    public string? Detail { get; set; }
}