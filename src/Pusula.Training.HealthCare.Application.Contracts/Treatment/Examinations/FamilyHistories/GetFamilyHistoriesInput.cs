using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public class GetFamilyHistoriesInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? MotherDisease { get; set; }
    public string? FatherDisease { get; set; }
    public string? SisterDisease { get; set; }
    public string? BrotherDisease { get; set; }
    public bool? AreParentsRelated { get; set; }
    public Guid? ExaminationId { get; set; }
    
    public GetFamilyHistoriesInput() { }
}