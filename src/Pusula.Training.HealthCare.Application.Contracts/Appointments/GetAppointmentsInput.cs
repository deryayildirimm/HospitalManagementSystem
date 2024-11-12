using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }

    public GetAppointmentsInput()
    {
    }
}