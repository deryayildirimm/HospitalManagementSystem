using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentViewModel
{
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public Enum Status { get; set; } = null!;
    public string Service { get; set; } = string.Empty;
}