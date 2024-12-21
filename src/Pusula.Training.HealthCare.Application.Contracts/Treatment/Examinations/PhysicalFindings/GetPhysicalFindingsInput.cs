using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public class GetPhysicalFindingsInput : PagedAndSortedResultRequestDto
{
    public int WeightMin { get; set; }
    public int WeightMax { get; set; }
    public int HeightMin { get; set; }
    public int HeightMax { get; set; }
    public Guid? ExaminationId { get; set; }
    public GetPhysicalFindingsInput() { }
}