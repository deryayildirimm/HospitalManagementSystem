using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypeDto: AuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string Name { get; set; } = null!;
    public string ConcurrencyStamp { get; set; } = null!;
}