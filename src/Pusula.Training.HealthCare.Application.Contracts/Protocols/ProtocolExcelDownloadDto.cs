using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Type { get; set; }
    public DateTime? StartTimeMin { get; set; }
    public DateTime? StartTimeMax { get; set; }
    public string? EndTime { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? DepartmentId { get; set; }

    public ProtocolExcelDownloadDto()
    {

    }
}