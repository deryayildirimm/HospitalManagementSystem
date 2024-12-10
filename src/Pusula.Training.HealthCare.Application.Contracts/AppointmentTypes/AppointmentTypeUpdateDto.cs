using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public class AppointmentTypeUpdateDto : IHasConcurrencyStamp
{
    [Required]
    [StringLength(AppointmentTypeConsts.NameMaxLength)]
    public string Name { get; set; } = null!;

    public string ConcurrencyStamp { get; set; } = null!;
}