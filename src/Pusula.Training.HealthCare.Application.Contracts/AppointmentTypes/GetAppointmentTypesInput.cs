using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class GetAppointmentTypesInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public string? Name { get; set; }

    public GetAppointmentTypesInput()
    {
    }
}