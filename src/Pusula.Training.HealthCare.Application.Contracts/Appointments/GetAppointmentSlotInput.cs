using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentSlotInput
{
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public DateTime Date { get; set; } = DateTime.Today;
    
    public bool ExcludeNotAvailable { get; set; } = false;
    
    public GetAppointmentSlotInput()
    {
    }
}