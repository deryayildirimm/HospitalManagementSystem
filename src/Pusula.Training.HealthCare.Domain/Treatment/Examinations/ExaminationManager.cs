using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationManager(
    IExaminationRepository examinationRepository,
    FamilyHistoryManager familyHistoryManager,
    BackgroundManager backgroundManager,
    PhysicalFindingManager physicalFindingManager) : DomainService
{
    public virtual async Task<Examination> CreateAsync(
        Guid protocolId,
        DateTime date,
        string complaint,
        bool areParentsRelated,
        string? motherDisease,
        string? fatherDisease,
        string? sisterDisease,
        string? brotherDisease,
        string? allergies,
        string? medications,
        string? habits,
        int? weight,
        int? height,
        int? bodyTemperature,
        int? pulse,
        int? vki,
        int? vya,
        int? kbs,
        int? kbd,
        int? spo2,
        DateTime? startDate,
        string? story,
        ICollection<Guid>? icdIds = null
    )
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        Check.NotNullOrWhiteSpace(complaint, nameof(complaint), ExaminationConsts.ComplaintMaxLength,
            ExaminationConsts.ComplaintMinLength);
        Check.Length(story, nameof(story), ExaminationConsts.StoryMaxLength);

        var examination = new Examination(
            Guid.NewGuid(),
            protocolId,
            date,
            complaint,
            startDate,
            story
        );

        var newExamination = await examinationRepository.InsertAsync(examination, true);

        await SetExaminationIcds(examination, icdIds);

        await familyHistoryManager.CreateAsync(newExamination.Id, areParentsRelated, motherDisease, fatherDisease,
            sisterDisease, brotherDisease);
        await backgroundManager.CreateAsync(newExamination.Id, allergies, medications, habits);
        await physicalFindingManager.CreateAsync(newExamination.Id, weight, height, bodyTemperature, pulse, vki, vya,
            kbs, kbd, spo2);

        return await examinationRepository.GetAsync(newExamination.Id);
    }

    private async Task SetExaminationIcds(Examination examination, IEnumerable<Guid>? icdKeys)
    {
        if (icdKeys == null || !icdKeys.Any())
        {
            await examinationRepository.UpdateExaminationIcdsAsync(examination.Id, new List<Guid>());
            return;
        }

        var icdIds = icdKeys.Distinct().ToList();
        await examinationRepository.UpdateExaminationIcdsAsync(examination.Id, icdIds);
    }

    public virtual async Task<Examination> UpdateAsync(
        Guid id,
        Guid protocolId,
        DateTime date,
        string complaint,
        bool areParentsRelated,
        string? motherDisease,
        string? fatherDisease,
        string? sisterDisease,
        string? brotherDisease,
        string? allergies,
        string? medications,
        string? habits,
        int? weight,
        int? height,
        int? bodyTemperature,
        int? pulse,
        int? vki,
        int? vya,
        int? kbs,
        int? kbd,
        int? spo2,
        DateTime? startDate,
        string? story,
        ICollection<Guid>? icdIds = null
    )
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        Check.NotNullOrWhiteSpace(complaint, nameof(complaint), ExaminationConsts.ComplaintMaxLength,
            ExaminationConsts.ComplaintMinLength);
        Check.Length(story, nameof(story), ExaminationConsts.StoryMaxLength);

        var examination = await examinationRepository.GetWithNavigationPropertiesAsync(id);

        Check.NotNull(examination, nameof(examination));

        examination.SetProtocolId(protocolId);
        examination.SetDate(date);
        examination.SetComplaint(complaint);
        examination.SetStartDate(startDate);
        examination.SetStory(story);

        await SetExaminationIcds(examination, icdIds);

        await familyHistoryManager.UpdateAsync(examination.FamilyHistory!.Id, id, areParentsRelated, motherDisease,
            fatherDisease, sisterDisease, brotherDisease);
        await backgroundManager.UpdateAsync(examination.Background!.Id, id, allergies, medications, habits);
        await physicalFindingManager.UpdateAsync(examination.PhysicalFinding!.Id, id, weight, height, 
            bodyTemperature, pulse, vki, vya, kbs, kbd, spo2);
        
        return await examinationRepository.UpdateAsync(examination);
    }
}