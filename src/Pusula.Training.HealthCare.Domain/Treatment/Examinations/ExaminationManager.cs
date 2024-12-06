using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationManager(IExaminationRepository examinationRepository) : DomainService
{
    public virtual async Task<Examination> CreateAsync(
        Guid protocolId,
        DateTime date,
        string complaint,
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
        return await examinationRepository.InsertAsync(examination);
    }

    public virtual async Task<Examination> UpdateAsync(
        Guid id,
        Guid protocolId,
        DateTime date,
        string complaint,
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