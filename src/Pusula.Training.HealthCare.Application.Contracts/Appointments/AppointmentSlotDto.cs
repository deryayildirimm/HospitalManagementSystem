using System;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentSlotDto
{
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public bool AvailabilityValue { get; set; }
    
}