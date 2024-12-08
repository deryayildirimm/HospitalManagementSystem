using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationUpdateDto
{
    [Required]
    public Guid Id { get; set; } = default!;
    [Required]
    public DateTime Date { get; set; } = DateTime.Now;
    [Required]
    [StringLength(ExaminationConsts.ComplaintMaxLength, MinimumLength = ExaminationConsts.ComplaintMinLength)]
    public string Complaint { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    [StringLength(ExaminationConsts.StoryMaxLength)]
    public string? Story { get; set; }

    public BackgroundUpdateDto Background { get; set; } = new BackgroundUpdateDto();
    public FamilyHistoryUpdateDto FamilyHistory { get; set; } = new FamilyHistoryUpdateDto();
    public ICollection<ExaminationIcdDto>? ExaminationIcd { get; set; }
    [Required]
    public Guid ProtocolId { get; set; }
}