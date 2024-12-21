using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public class PhysicalFindingUpdateDto
{
    [Required]
    public Guid Id { get; set; } = default!;
    [Range(PhysicalFindingConsts.WeightMinValue, PhysicalFindingConsts.WeightMaxValue)]
    public int? Weight { get; set; }
    [Range(PhysicalFindingConsts.HeightMinValue, PhysicalFindingConsts.HeightMaxValue)]
    public int? Height { get; set; }
    public int? BodyTemperature { get; set; }
    public int? Pulse { get; set; }
    public int? Vki { get; set; }
    public int? Vya { get; set; }
    public int? Kbs { get; set; }
    public int? Kbd { get; set; }
    public int? Spo2 { get; set; }
    [Required]
    public Guid ExaminationId { get; set; }
}