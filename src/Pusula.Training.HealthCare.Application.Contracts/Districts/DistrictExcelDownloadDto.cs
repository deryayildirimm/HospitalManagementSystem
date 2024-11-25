using System;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictExcelDownloadDto
{
    public string DownloadToken { get; set; } = null!;
    public string? FilterText { get; set; }
    public string? Name { get; set; }
    public Guid? CityId { get; set; }
}