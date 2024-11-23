using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Districts;

public class GetDistrictsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? Name { get; set; }
    public Guid? CityId { get; set; }
    
    public GetDistrictsInput() { }
}