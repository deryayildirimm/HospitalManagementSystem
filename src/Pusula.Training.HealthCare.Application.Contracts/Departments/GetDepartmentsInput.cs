using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Departments;

public class GetDepartmentsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public GetDepartmentsInput()
    {
    }
}