using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class ExaminationCreateDto
{
    [Required] public DateTime Date { get; set; } = DateTime.Now;
    [Required]
    [StringLength(ExaminationConsts.ComplaintMaxLength, MinimumLength = ExaminationConsts.ComplaintMinLength)]
    public string Complaint { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    [StringLength(ExaminationConsts.StoryMaxLength)]
    public string? Story { get; set; }
    [Required]
    public Guid ProtocolId { get; set; }
    [Required]
    public FamilyHistoryCreateInExaminationDto FamilyHistory { get; set; } = new FamilyHistoryCreateInExaminationDto();
    [Required]
    public BackgroundCreateInExaminationDto Background { get; set; } = new BackgroundCreateInExaminationDto();
    [Required]
    public PhysicalFindingCreateInExaminationDto PhysicalFinding { get; set; } = new PhysicalFindingCreateInExaminationDto();
    public List<Guid> IcdIds { get; set; } = new List<Guid>();
}