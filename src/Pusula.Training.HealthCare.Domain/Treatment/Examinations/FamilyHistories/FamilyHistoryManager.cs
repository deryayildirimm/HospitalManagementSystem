using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class FamilyHistoryManager(IFamilyHistoryRepository familyHistoryRepository) : DomainService
{
    public virtual async Task<FamilyHistory> CreateAsync(
        Guid examinationId,
        bool areParentsRelated,
        string? motherDisease,
        string? fatherDisease,
        string? sisterDisease,
        string? brotherDisease)
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        Check.NotNull(areParentsRelated, nameof(areParentsRelated));
        Check.Length(motherDisease, nameof(motherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(fatherDisease, nameof(fatherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(sisterDisease, nameof(sisterDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(brotherDisease, nameof(brotherDisease), FamilyHistoryConsts.DiseaseMaxLength);

        var familyHistory = new FamilyHistory(
            Guid.NewGuid(),
            examinationId, 
            areParentsRelated, 
            motherDisease, 
            fatherDisease, 
            sisterDisease, 
            brotherDisease
        );
        return await familyHistoryRepository.InsertAsync(familyHistory);
    }

    public virtual async Task<FamilyHistory> UpdateAsync(
        Guid id,
        Guid examinationId,
        bool areParentsRelated,
        string? motherDisease,
        string? fatherDisease,
        string? sisterDisease,
        string? brotherDisease
    )
    {
        Check.NotNullOrWhiteSpace(examinationId.ToString(), nameof(examinationId));
        Check.NotNull(areParentsRelated, nameof(areParentsRelated));
        Check.Length(motherDisease, nameof(motherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(fatherDisease, nameof(fatherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(sisterDisease, nameof(sisterDisease), FamilyHistoryConsts.DiseaseMaxLength);
        Check.Length(brotherDisease, nameof(brotherDisease), FamilyHistoryConsts.DiseaseMaxLength);
        
        var familyHistory = await familyHistoryRepository.GetAsync(id);
        
        familyHistory.SetMotherDisease(motherDisease);
        familyHistory.SetFatherDisease(fatherDisease);
        familyHistory.SetSisterDisease(sisterDisease);
        familyHistory.SetBrotherDisease(brotherDisease);
        familyHistory.SetAreParentsRelated(areParentsRelated);
        familyHistory.SetExaminationId(examinationId);
        
        return await familyHistoryRepository.UpdateAsync(familyHistory);
    }
}