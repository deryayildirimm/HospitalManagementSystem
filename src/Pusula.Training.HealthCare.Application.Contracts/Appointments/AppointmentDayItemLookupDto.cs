using System;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentDayItemLookupDto
{
    public DateTime Date { get; set; }
    public int AvailableSlotCount { get; set; } = 0;
    public bool AvailabilityValue { get; set; } = false;
    public bool IsSelected { get; set; }
}