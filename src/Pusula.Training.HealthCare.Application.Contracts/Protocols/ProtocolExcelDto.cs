using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolExcelDto
{
    public string Type { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public string? EndTime { get; set; }
}