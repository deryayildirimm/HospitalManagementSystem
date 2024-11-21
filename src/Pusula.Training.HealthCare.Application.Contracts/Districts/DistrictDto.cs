using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Districts;

public class DistrictDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
    public Guid CityId { get; set; }
}