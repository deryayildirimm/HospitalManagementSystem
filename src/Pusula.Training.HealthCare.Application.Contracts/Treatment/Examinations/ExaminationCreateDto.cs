using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationCreateDto
{
    [Required]
    public DateTime Date { get; set; }
    [Required]
    [StringLength(ExaminationConsts.ComplaintMaxLength, MinimumLength = ExaminationConsts.ComplaintMinLength)]
    public string Complaint { get; set; } = null!;
    public DateTime? StartDate { get; set; }
    [StringLength(ExaminationConsts.StoryMaxLength)]
    public string? Story { get; set; }
    [Required]
    public Guid ProtocolId { get; set; }
}