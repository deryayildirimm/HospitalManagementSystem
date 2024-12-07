using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentsViewedEto : EtoBase
{
    public Guid Id { get; set; }

    public DateTime ViewedAt { get; set; }
}
