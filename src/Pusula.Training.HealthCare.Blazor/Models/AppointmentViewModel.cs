using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentViewModel
{
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public Enum Status { get; set; } 
    public string Service { get; set; } = string.Empty;
}