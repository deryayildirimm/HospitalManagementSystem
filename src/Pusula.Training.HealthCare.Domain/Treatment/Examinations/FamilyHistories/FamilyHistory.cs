using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class FamilyHistory : FullAuditedAggregateRoot<Guid>
{
    public string? MotherDisease { get; protected set; }
    public string? FatherDisease { get; protected set; }
    public string? SisterDisease { get; protected set; }
    public string? BrotherDisease { get; protected set; }
    [NotNull]
    public bool AreParentsRelated { get; protected set; }
    public Guid ExaminationId { get; protected set; }
    public Examination Examination { get; set; }

    protected FamilyHistory()
    {
        AreParentsRelated = false;
        ExaminationId = Guid.Empty;
    }

    public FamilyHistory(Guid id, Guid examinationId, bool areParentsRelated, string? motherDisease = null, 
        string? fatherDisease = null, string? sisterDisease = null, string? brotherDisease = null)
    {
        Id = id;
        SetMotherDisease(motherDisease);
        SetFatherDisease(fatherDisease);
        SetSisterDisease(sisterDisease);
        SetBrotherDisease(brotherDisease);
        SetAreParentsRelated(areParentsRelated);
        SetExaminationId(examinationId);
    }

    public void SetMotherDisease(string? motherDisease)
    {
        Check.Length(motherDisease, nameof(motherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        MotherDisease = motherDisease;
    }

    public void SetFatherDisease(string? fatherDisease)
    {
        Check.Length(fatherDisease, nameof(fatherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        FatherDisease = fatherDisease;
    }

    public void SetSisterDisease(string? sisterDisease)
    {
        Check.Length(sisterDisease, nameof(sisterDisease), FamilyHistoryConsts.DiseaseMaxLength);
        SisterDisease = sisterDisease;
    }

    public void SetBrotherDisease(string? brotherDisease)
    {
        Check.Length(brotherDisease, nameof(brotherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        BrotherDisease = brotherDisease;
    }

    public void SetAreParentsRelated(bool areParentsRelated)
    {
        Check.NotNull(areParentsRelated, nameof(areParentsRelated));
        AreParentsRelated = areParentsRelated;
    }
    
    public void SetExaminationId(Guid examinationId)
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        ExaminationId = examinationId;
    }
}