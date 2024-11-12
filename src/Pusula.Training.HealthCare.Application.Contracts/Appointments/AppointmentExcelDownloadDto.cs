namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public AppointmentExcelDownloadDto()
    {
    }
}