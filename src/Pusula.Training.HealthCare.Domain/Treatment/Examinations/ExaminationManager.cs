using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationManager(IExaminationRepository examinationRepository,
    FamilyHistoryManager familyHistoryManager,
    BackgroundManager backgroundManager,
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
        string? story)
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
        await unitOfWorkManager.Current.SaveChangesAsync();

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
        string? story
    )
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        Check.NotNullOrWhiteSpace(complaint, nameof(complaint), ExaminationConsts.ComplaintMaxLength, 
            ExaminationConsts.ComplaintMinLength);
        Check.Length(story, nameof(story), ExaminationConsts.StoryMaxLength);
        
        var examination = await examinationRepository.GetAsync(id);
        
        examination.SetProtocolId(protocolId);
        examination.SetDate(date);
        examination.SetComplaint(complaint);
        examination.SetStartDate(startDate);
        examination.SetStory(story);
        
        return await examinationRepository.UpdateAsync(examination);
    }
}