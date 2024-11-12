using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentUpdateDto : IHasConcurrencyStamp
{
    [Required]
    public string Name { get; set; } = null!;

    public string ConcurrencyStamp { get; set; } = null!;
}