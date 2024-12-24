using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public class ProtocolTypesViewedEto : EtoBase
{
    public Guid Id { get; set; }

    public DateTime ViewedAt { get; set; }
}