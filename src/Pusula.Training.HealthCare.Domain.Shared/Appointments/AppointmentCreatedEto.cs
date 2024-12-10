using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentCreatedEto : EtoBase
{
    public Guid Id { get; set; }    
    public DateTime CreatedAt { get; set; }
}