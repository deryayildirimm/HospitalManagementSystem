using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceUpdateDto : IHasConcurrencyStamp
{
    [Required]
    public string Name { get; set; }
    
    [Required]
    public List<string> DepartmentNames { get; set; } = new List<string>();

    [Required]
    [Range(MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue)]
    public double Cost { get; set; }

    public DateTime ServiceCreatedAt { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}