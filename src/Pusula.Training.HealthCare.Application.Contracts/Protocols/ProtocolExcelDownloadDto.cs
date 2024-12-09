using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;

    public string? FilterText { get; set; }

    public string? Type { get; set; }
    public DateTime? StartTimeMin { get; set; }
    public DateTime? StartTimeMax { get; set; }
    public DateTime? EndTimeMin { get; set; }
    public DateTime? EndTimeMax { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? DepartmentId { get; set; }
    
    public Guid? ProtocolTypeId { get; set; }
    
    public Guid? DoctorId { get; set; }

    public Guid? InsuranceId { get; set; }
    public ProtocolExcelDownloadDto()
    {

    }
}