using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentDayItem
{
    public DateTime Date { get; set; }
    public bool IsSelected { get; set; }
    public bool IsAvailable { get; set; }
}