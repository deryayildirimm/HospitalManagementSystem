namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdReportDto
{
    public string CodeNumber { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public int Quantity { get; set; }
}