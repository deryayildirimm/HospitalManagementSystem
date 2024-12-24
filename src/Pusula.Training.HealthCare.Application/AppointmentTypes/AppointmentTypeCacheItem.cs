using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeCacheItem : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = null!;
}