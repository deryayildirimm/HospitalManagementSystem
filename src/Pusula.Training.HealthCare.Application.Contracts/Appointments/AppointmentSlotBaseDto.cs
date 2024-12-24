using System;
using System.ComponentModel.DataAnnotations;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentSlotBaseDto
{
    public DateTime Date { get; set; }
    
    public string StartTime { get; set; } = null!;
    
    public string EndTime { get; set; } = null!;
}