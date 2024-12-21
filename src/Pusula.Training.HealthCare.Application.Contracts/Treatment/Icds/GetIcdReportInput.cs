using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public class GetIcdReportInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? CodeNumber { get; set; }
    public string? Detail { get; set; }
}