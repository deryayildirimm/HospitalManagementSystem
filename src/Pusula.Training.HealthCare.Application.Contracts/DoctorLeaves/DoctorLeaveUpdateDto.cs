using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Validators;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveUpdateDto : IHasConcurrencyStamp
{
    [Required(ErrorMessage = "Error:GuidRequired")]
    [GuidValidator(ErrorMessage = "Error:GuidRequired")]
    public Guid DoctorId { get; set; }

    [Required] public virtual DateTime StartDate { get; set; }

    [Required] public virtual DateTime EndDate { get; set; }

    [Required]
    [Range(DoctorLeaveConsts.TypeMinValue, DoctorLeaveConsts.TypeMaxValue)]
    public virtual EnumLeaveType EnumLeaveType { get; set; }

    [MaxLength(DoctorLeaveConsts.TextMaxLength, ErrorMessage = "Error:TextLengthExceeded")]
    public virtual string? Reason { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}