using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Titles;

public class TitleDto : AuditedEntityDto<Guid>
{
    public string TitleName { get; set; } = null!;
}