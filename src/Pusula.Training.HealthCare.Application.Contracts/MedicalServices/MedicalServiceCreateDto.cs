using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceCreateDto
{
    [Required] 
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(MedicalServiceConsts.DurationMinValue, MedicalServiceConsts.DurationMaxValue)]
    public int Duration { get; set; }
    
    [Required]
    public readonly List<string> DepartmentNames = [];

    [Required]
    [Range(MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue)]
    public double Cost { get; set; } = 0;

    [Required] public DateTime ServiceCreatedAt { get; set; } = DateTime.Now;
}