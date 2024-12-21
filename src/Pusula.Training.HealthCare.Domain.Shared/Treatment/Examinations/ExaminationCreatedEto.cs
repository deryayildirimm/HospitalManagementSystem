using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationCreatedEto : EtoBase
{
    public Guid Id { get; set; }
    public Guid? FamilyHistoryId { get; set; }
    public Guid? BackgroundId { get; set; }
    public DateTime CreatedAt { get; set; }
}