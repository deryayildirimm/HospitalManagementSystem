using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationDto : FullAuditedEntityDto<Guid>
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    [StringLength(ExaminationConsts.ComplaintMaxLength, MinimumLength = ExaminationConsts.ComplaintMinLength)]
    public string Complaint { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    [StringLength(ExaminationConsts.StoryMaxLength)]
    public string? Story { get; set; }
    public BackgroundDto? Background { get; set; }
    public FamilyHistoryDto? FamilyHistory { get; set; }
    public List<ExaminationIcdDto> ExaminationIcd { get; set; } = null!;
    [Required]
    public Guid ProtocolId { get; set; }
    public ProtocolDto Protocol { get; set; } = null!;
}