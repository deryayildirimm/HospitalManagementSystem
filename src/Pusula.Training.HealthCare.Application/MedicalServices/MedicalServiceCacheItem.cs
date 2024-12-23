using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceCacheItem : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public double Cost { get; set; }
    public int Duration { get; set; }
    public DateTime ServiceCreatedAt { get; set; }
}