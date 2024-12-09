using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationManager(
    IExaminationRepository examinationRepository,
    FamilyHistoryManager familyHistoryManager,
    BackgroundManager backgroundManager,
    IIcdRepository icdRepository,
    IUnitOfWorkManager unitOfWorkManager) : DomainService
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
        DateTime? startDate,
        string? story,
        List<Guid>? icdIds = null
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
        
        var newExamination = await examinationRepository.InsertAsync(examination);
        await unitOfWorkManager.Current!.SaveChangesAsync();
        
        var icds = await icdRepository.GetListAsync(x => icdIds.Contains(x.Id));
        foreach (var icd in icds)
        {
            var examinationIcd = new ExaminationIcd
            {
                ExaminationId = examination.Id,
                IcdId = icd.Id,
            };
            examination.ExaminationIcd.Add(examinationIcd);
        }
        
        await familyHistoryManager.CreateAsync(newExamination.Id, areParentsRelated, motherDisease, fatherDisease,
            sisterDisease, brotherDisease);
        await backgroundManager.CreateAsync(newExamination.Id, allergies, medications, habits);

        return await examinationRepository.GetAsync(newExamination.Id);
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
        DateTime? startDate,
        string? story,
        List<Guid>? icdIds = null
    )
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        Check.NotNullOrWhiteSpace(complaint, nameof(complaint), ExaminationConsts.ComplaintMaxLength,
            ExaminationConsts.ComplaintMinLength);
        Check.Length(story, nameof(story), ExaminationConsts.StoryMaxLength);

        var examination = await examinationRepository.GetWithNavigationPropertiesAsync(id);

        examination!.SetProtocolId(protocolId);
        examination!.SetDate(date);
        examination!.SetComplaint(complaint);
        examination!.SetStartDate(startDate);
        examination!.SetStory(story);
        
        var icds = await icdRepository.GetListAsync(x => icdIds.Contains(x.Id));
        foreach (var icd in icds)
        {
            var examinationIcd = new ExaminationIcd
            {
                ExaminationId = examination.Id,
                IcdId = icd.Id,
            };
            examination.ExaminationIcd.Add(examinationIcd);
        }
        await familyHistoryManager.UpdateAsync(examination.FamilyHistory!.Id, id, areParentsRelated, motherDisease,
            fatherDisease, sisterDisease, brotherDisease);
        await backgroundManager.UpdateAsync(examination.Background!.Id, id, allergies, medications, habits);

        return await examinationRepository.UpdateAsync(examination);
    }
}