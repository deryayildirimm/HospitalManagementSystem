using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveViewedEto : EtoBase
{
    public Guid Id { get; set; }

    public DateTime ViewedAt { get; set; }
}