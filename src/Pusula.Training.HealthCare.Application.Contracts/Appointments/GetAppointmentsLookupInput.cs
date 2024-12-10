using System;
using System.ComponentModel.DataAnnotations;
using Pusula.Training.HealthCare.Validators;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentsLookupInput
{
    [Required]
    public Guid DoctorId { get; set; }
    
    [Required]
    public Guid MedicalServiceId { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Range(AppointmentConsts.MinOffset, AppointmentConsts.MaxOffset)]
    public int Offset { get; set; }
}