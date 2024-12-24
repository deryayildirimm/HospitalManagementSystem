using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public class PhysicalFinding : FullAuditedAggregateRoot<Guid>
{
    public int? Weight { get; private set; }
    public int? Height { get; private set; }
    public int? BodyTemperature { get; private set; }
    public int? Pulse { get; private set; }
    public int? Vki { get; private set; }
    public int? Vya { get; private set; }
    public int? Kbs { get; private set; }
    public int? Kbd { get; private set; }
    public int? Spo2 { get; private set; }
    [NotNull]
    public Guid ExaminationId { get; private set; }
    public Examination Examination { get; private set; } = null!;

    public PhysicalFinding(Guid id, Guid examinationId, int? weight = null, int? height = null, int? bodyTemperature = null, int? pulse = null, 
        int? vki = null, int? vya = null, int? kbs = null, int? kbd = null, int? spo2 = null)
    {
        Id = id;
        SetExaminationId(examinationId);
        SetWeight(weight);
        SetHeight(height);
        SetBodyTemperature(bodyTemperature);
        SetPulse(pulse);
        SetVki(vki);
        SetVya(vya);
        SetKbs(kbs);
        SetKbd(kbd);
        SetSpo2(spo2);
    }

    public void SetWeight(int? weight)
    {
        HealthCareGlobalException.ThrowIf("Weight is out of range", 
            weight.HasValue && weight < PhysicalFindingConsts.WeightMinValue || weight > PhysicalFindingConsts.WeightMaxValue);
        Weight = weight;
    }

    public void SetHeight(int? height)
    {
        HealthCareGlobalException.ThrowIf("Height is out of range", 
            height.HasValue && height < PhysicalFindingConsts.HeightMinValue || height > PhysicalFindingConsts.HeightMaxValue);
        Height = height;
    }

    public void SetBodyTemperature(int? bodyTemperature)
    {
        BodyTemperature = bodyTemperature;
    }

    public void SetPulse(int? pulse)
    {
        Pulse = pulse;
    }

    public void SetVki(int? vki)
    {
        Vki = vki;
    }

    public void SetVya(int? vya)
    {
        Vya = vya;
    }

    public void SetKbs(int? kbs)
    {
        Kbs = kbs;
    }

    public void SetKbd(int? kbd)
    {
        Kbd = kbd;
    }

    public void SetSpo2(int? spo2)
    {
        Spo2 = spo2;
    }

    public void SetExaminationId(Guid examinationId)
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        ExaminationId = examinationId;
    }
}