using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class FamilyHistoryUpdateDto
{
    [Required]
    public Guid Id { get; set; } = default!;
    [StringLength(FamilyHistoryConsts.DiseaseMaxLength)]
    public string? MotherDisease { get; set; }
    [StringLength(FamilyHistoryConsts.DiseaseMaxLength)]
    public string? FatherDisease { get; set; }
    [StringLength(FamilyHistoryConsts.DiseaseMaxLength)]
    public string? SisterDisease { get; set; }
    [StringLength(FamilyHistoryConsts.DiseaseMaxLength)]
    public string? BrotherDisease { get; set; }
    [Required]
    public bool AreParentsRelated { get; set; } = false;
    [Required]
    public Guid ExaminationId { get; set; }
}