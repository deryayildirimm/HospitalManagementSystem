using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations;

public class GetExaminationsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public DateTime? DateMin { get; set; }
    public DateTime? DateMax { get; set; }
    public string? Complaint { get; set; }
    public string? Story { get; set; }
    public Guid? ProtocolId { get; set; }
    
    public GetExaminationsInput() { }
}