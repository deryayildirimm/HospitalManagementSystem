using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceDto : FullAuditedEntityDto<Guid>
{
    [Required] 
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue)]
    public double Cost { get; set; } = 0;

    [Required]
    public DateTime ServiceCreatedAt { get; set; } = DateTime.Now;
    
}