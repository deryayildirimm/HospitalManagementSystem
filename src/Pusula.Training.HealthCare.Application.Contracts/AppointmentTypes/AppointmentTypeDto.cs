using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeDto : FullAuditedEntityDto<Guid>
{
    [Required]
    [StringLength(AppointmentTypeConsts.NameMaxLength)]
    public string Name { get; set; } = null!;
}