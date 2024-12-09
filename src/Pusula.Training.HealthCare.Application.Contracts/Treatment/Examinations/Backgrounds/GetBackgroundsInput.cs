using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public class GetBackgroundsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Allergies { get; set; }
    public string? Medications { get; set; }
    public string? Habits { get; set; }
    public Guid? ExaminationId { get; set; }
    
    public GetBackgroundsInput() { }
}