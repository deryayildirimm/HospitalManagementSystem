using Volo.Abp.Application.Dtos;
using System;

namespace Pusula.Training.HealthCare.Protocols;

public class GetProtocolsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Type { get; set; }
    public DateTime? StartTimeMin { get; set; }
    public DateTime? StartTimeMax { get; set; }
    public string? EndTime { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? DepartmentId { get; set; }

    public GetProtocolsInput()
    {
    }
}