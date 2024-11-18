using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public class DoctorLeaveUpdateDto : IHasConcurrencyStamp
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required] 
    public virtual DateTime StartDate { get; set; } 
    
    [Required]
    public virtual DateTime EndDate { get; set; } 
   
    public virtual string? Reason { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}