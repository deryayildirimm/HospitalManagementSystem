using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.DoctorLeaves;


public class DoctorLeaveDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{

    [Required]
    public Guid DoctorId { get; set; } = Guid.Empty;

    public DoctorDto Doctor { get; set; } = null!;
    [Required]
    public virtual DateTime StartDate { get; set; } = DateTime.Today;
    [Required]
    public virtual DateTime EndDate { get; set; } = DateTime.Today;
    
    public virtual EnumLeaveType LeaveType { get; set; }
   
    public virtual string? Reason { get; set; }
    
    public string ConcurrencyStamp { get; set; } = null!;
}