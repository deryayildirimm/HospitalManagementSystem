namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public AppointmentTypeExcelDownloadDto()
    {
    }
}