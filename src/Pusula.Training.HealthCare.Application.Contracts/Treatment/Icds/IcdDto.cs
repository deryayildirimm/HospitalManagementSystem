using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class IcdDto : FullAuditedEntityDto<Guid>
{
    [Required]
    [StringLength(IcdConsts.CodeNumberMaxLength, MinimumLength = IcdConsts.CodeNumberMinLength)]
    public string CodeNumber { get; set; } = null!;
    [Required]
    [StringLength(IcdConsts.DetailMaxLength, MinimumLength = IcdConsts.DetailMinLength)]
    public string Detail { get; set; } = null!;
}