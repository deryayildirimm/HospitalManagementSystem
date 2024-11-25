using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class AppointmentDayLookupItem
{
    public DateTime Date { get; set; }
    public int AvailableSlotCount { get; set; } = 0;
    public bool AvailabilityValue { get; set; } = false;
    public bool IsSelected { get; set; }
}