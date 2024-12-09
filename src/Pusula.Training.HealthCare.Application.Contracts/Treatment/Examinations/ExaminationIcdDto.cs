using System;
using System.Text.Json.Serialization;
using Pusula.Training.HealthCare.Treatment.Icds;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationIcdDto
{
    public Guid ExaminationId { get; set; }
    [JsonIgnore]
    public ExaminationDto Examination { get; set; } = null!;
    
    public Guid IcdId { get; set; }
    public IcdDto Icd { get; set; } = null!;
}