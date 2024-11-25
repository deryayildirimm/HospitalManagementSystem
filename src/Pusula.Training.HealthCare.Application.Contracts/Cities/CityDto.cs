using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Cities;

public class CityDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
}