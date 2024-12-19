using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Validators;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveCreateDto
{
    [Required(ErrorMessage="Error:GuidRequired")] 
    [GuidValidator(ErrorMessage="Error:GuidRequired")]
    public Guid DoctorId { get; set; }

    [Required] public virtual DateTime StartDate { get; set; } = DateTime.Today;

    [Required] public virtual DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    [Required]
    [Range(DoctorLeaveConsts.TypeMinValue, DoctorLeaveConsts.TypeMaxValue)]
    public virtual EnumLeaveType EnumLeaveType { get; set; }
    
    [MaxLength(DoctorLeaveConsts.TextMaxLength, ErrorMessage = "Error:TextLengthExceeded")]
    public virtual string? Reason { get; set; }
}