using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class BackgroundDto : FullAuditedEntityDto<Guid>
{
    [StringLength(BackgroundConsts.AllergiesMaxLength)]
    public string? Allergies { get; set; }
    [StringLength(BackgroundConsts.MedicationsMaxLength)]
    public string? Medications { get; set; }
    [StringLength(BackgroundConsts.HabitsMaxLength)]
    public string? Habits { get; set; }
    [Required]
    public Guid ExaminationId { get; set; }
    [JsonIgnore]
    public ExaminationDto Examination { get; set; } = null!;
}