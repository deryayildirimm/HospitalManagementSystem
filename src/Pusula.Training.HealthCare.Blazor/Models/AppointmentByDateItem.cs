using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentByDateItem
{
    public string GroupKey { get; set; } = string.Empty;
    public DateTime GroupName { get; set; }
    public int Number { get; set; }
}