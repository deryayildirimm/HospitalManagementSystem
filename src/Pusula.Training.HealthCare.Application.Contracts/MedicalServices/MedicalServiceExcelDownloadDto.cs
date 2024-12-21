using System;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;
    public Guid MedicalServiceId { get; set; }
    public Guid? DepartmentId { get; set; }

    public Guid? PatientId { get; set; }
    public DateTime? ServiceDateMin { get; set; }
    public DateTime? ServiceDateMax { get; set; }
    
    public double? CostMin { get; set; }
    public double? CostMax { get; set; }
    public string? MedicalServiceName { get; set; }

    public MedicalServiceExcelDownloadDto()
    {
    }
}