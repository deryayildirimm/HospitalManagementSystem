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

        await AddExaminationIcd(icdIds, examination);

        await familyHistoryManager.CreateAsync(newExamination.Id, areParentsRelated, motherDisease, fatherDisease,
            sisterDisease, brotherDisease);
        await backgroundManager.CreateAsync(newExamination.Id, allergies, medications, habits);

        return await examinationRepository.GetAsync(newExamination.Id);
    }

    private async Task AddExaminationIcd(List<Guid>? icdIds, Examination examination)
    {
        if (icdIds != null && icdIds.Count != 0)
        {
            var icds = await icdRepository.GetListAsync(x => icdIds.Contains(x.Id));
            foreach (var icd in icds)
            {
                examination.AddIcd(new ExaminationIcd
                {
                    ExaminationId = examination.Id,
                    IcdId = icd.Id,
                });
            }
        }
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

        Check.NotNull(examination, nameof(examination));

        examination.SetProtocolId(protocolId);
        examination.SetDate(date);
        examination.SetComplaint(complaint);
        examination.SetStartDate(startDate);
        examination.SetStory(story);

        await UpdateExaminationIcds(icdIds, examination);

        await familyHistoryManager.UpdateAsync(examination.FamilyHistory!.Id, id, areParentsRelated, motherDisease,
            fatherDisease, sisterDisease, brotherDisease);
        await backgroundManager.UpdateAsync(examination.Background!.Id, id, allergies, medications, habits);

        return await examinationRepository.UpdateAsync(examination);
    }

    private async Task UpdateExaminationIcds(List<Guid>? icdIds, Examination examination)
    {
        var currentIcdIds = examination.ExaminationIcd.Select(e => e.IcdId).ToList();
        var newIcdIds = icdIds ?? new List<Guid>();

        foreach (var icdId in currentIcdIds.Where(icdId => !newIcdIds.Contains(icdId)))
        {
            examination.RemoveIcd(icdId);
        }

        var icdsToAdd = await icdRepository.GetListAsync(x => newIcdIds.Contains(x.Id) && !currentIcdIds.Contains(x.Id));
        foreach (var icd in icdsToAdd)
        {
            examination.AddIcd(new ExaminationIcd
            {
                ExaminationId = examination.Id,
                IcdId = icd.Id,
            });
        }
    }
}