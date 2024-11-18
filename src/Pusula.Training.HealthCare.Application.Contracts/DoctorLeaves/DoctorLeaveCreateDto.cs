using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveCreateDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required] public virtual DateTime StartDate { get; set; } 
    [Required]
    public virtual DateTime EndDate { get; set; } 
   
    public virtual string? Reason { get; set; }
    
   
}