using System;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationIcd : Entity
{
    public Guid ExaminationId { get; set; }
    public Examination Examination { get; set; } = null!;
    
    public Guid IcdId { get; set; }
    public Icd Icd { get; set; } = null!;

    public override object?[] GetKeys() => new object?[] { ExaminationId, IcdId };
}