using System;
using System.Collections.Generic;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class Examination : FullAuditedAggregateRoot<Guid>
{
    public DateTime Date { get; protected set; }
    public string Complaint { get; protected set; }
    public DateTime? StartDate { get; protected set; }
    public string? Story { get; protected set; }
    public Background? Background { get; protected set; }
    public FamilyHistory? FamilyHistory { get; protected set; }
    public ICollection<ExaminationIcd>? ExaminationIcd { get; protected set; }
    public Guid ProtocolId { get; protected set; }
    public Protocol Protocol { get; protected set; }

    protected Examination()
    {
        Date = DateTime.Now;
        Complaint = string.Empty;
    }

    public Examination(Guid id, Guid protocolId, DateTime date, string complaint, DateTime? startDate, string? story, 
        ICollection<ExaminationIcd>? examinationIcd)
    {
        Id = id;
        SetProtocolId(protocolId);
        SetDate(date);
        SetComplaint(complaint);
        SetStartDate(startDate);
        SetStory(story);
        SetExaminationIcd(examinationIcd);
    }

    public void SetDate(DateTime date) => Date = date;
    
    public void SetComplaint(string complaint)
    {
        Check.NotNullOrWhiteSpace(complaint, nameof(complaint), ExaminationConsts.ComplaintMaxLength, 
            ExaminationConsts.ComplaintMinLength);
        Complaint = complaint;
    }
    
    public void SetStartDate(DateTime? startDate) => StartDate = startDate;
    
    public void SetStory(string? story)
    {
        Check.Length(story, nameof(story), ExaminationConsts.StoryMaxLength);
        Story = story;
    }
    
    public void SetExaminationIcd(ICollection<ExaminationIcd>? examinationIcd) => ExaminationIcd = examinationIcd;
    
    public void SetProtocolId(Guid protocolId)
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        ProtocolId = protocolId;
    }
}