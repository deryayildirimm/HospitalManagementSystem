using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Appointments;

public class ApointmentUpdatedEto: EtoBase
{
    public Guid Id { get; set; }

    public DateTime UpdatedAt { get; set; }
}