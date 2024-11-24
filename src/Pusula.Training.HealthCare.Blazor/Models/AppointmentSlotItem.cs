using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentSlotItem
{
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool AvailabilityValue { get; set; }
    public bool IsSelected { get; set; } = false;
}