using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentCustomData
{
    public Guid Id { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime DateOnly { get; set; }
    public DateTime HourOnly { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsAllDay { get; set; }
    public bool IsReadOnly { get; set; }
}