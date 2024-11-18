using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentDto : FullAuditedEntityDto<Guid>
{
    public Guid DoctorId { get; set; }
    
    public Guid MedicalServiceId { get; set; }
    
    public DateTime AppointmentDate { get; set; }
    
    public string AppointmentTime { get; set; } = null!;
    
    public string Availability {get; set;} = null!;
    
    public bool AvailabilityValue {get; set;} = false;

    public AppointmentDto()
    {
    }
}