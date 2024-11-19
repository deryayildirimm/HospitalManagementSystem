using System;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;
    
    public string? FilterText { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime? StartDateMin { get; set; }
    public DateTime? StartDateMax { get; set; }
    public DateTime? EndDateMin { get; set; }
    public DateTime? EndDateMax { get; set; }
    public string? Reason { get; set; }

    public DoctorLeaveExcelDownloadDto()
    {
        
    }

}