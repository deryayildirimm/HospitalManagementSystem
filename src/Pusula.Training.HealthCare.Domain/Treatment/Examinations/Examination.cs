using System;
using System.Collections.Generic;
using System.Linq;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class Examination : FullAuditedAggregateRoot<Guid>
{
    public DateTime Date { get; private set; }
    public string Complaint { get; private set; } = null!;
    public DateTime? StartDate { get; private set; }
    public string? Story { get; private set; }
    public Background? Background { get; private set; }
    public FamilyHistory? FamilyHistory { get; private set; }
    public PhysicalFinding? PhysicalFinding { get; private set; }
    public ICollection<ExaminationIcd> ExaminationIcds { get; private set; }
    public Guid ProtocolId { get; private set; }
    public Protocol Protocol { get; private set; } = null!;

    protected Examination()
    {
        Date = DateTime.Now;
        Complaint = string.Empty;
        ExaminationIcds = [];
    }

    public Examination(Guid id, Guid protocolId, DateTime date, string complaint, DateTime? startDate, string? story)
    {
        Id = id;
        SetProtocolId(protocolId);
        SetDate(date);
        SetComplaint(complaint);
        SetStartDate(startDate);
        SetStory(story);
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
    
    public void SetProtocolId(Guid protocolId)
    {
        Check.NotNullOrWhiteSpace(protocolId.ToString(), nameof(protocolId));
        ProtocolId = protocolId;
    }
    
    public void AddIcd(Guid icdId)
    {
        Check.NotNull(icdId, nameof(icdId));

        if (IsInIcd(icdId)) return;
            
        ExaminationIcds.Add(new ExaminationIcd(Id, icdId));
    }

    public void RemoveIcd(Guid icdId)
    {
        Check.NotNull(icdId, nameof(icdId));

        if (!IsInIcd(icdId)) return;

        ExaminationIcds.RemoveAll(x => x.IcdId == icdId);
    }

    public void RemoveAllIcdsExceptGivenIds(ICollection<Guid> icdIds)
    {
        Check.NotNullOrEmpty(icdIds, nameof(icdIds));
        ExaminationIcds.RemoveAll(x => !icdIds.Contains(x.IcdId));
    }

    public void RemoveAllIcds() => ExaminationIcds.RemoveAll(x => x.ExaminationId == Id);

    private bool IsInIcd(Guid icdId) => ExaminationIcds.Any(x => x.IcdId == icdId);
}