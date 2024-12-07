﻿using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class FamilyHistoryCreateInExaminationDto
{
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
}