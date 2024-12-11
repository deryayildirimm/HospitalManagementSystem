namespace Pusula.Training.HealthCare.Appointments;

public class GroupedAppointmentCountDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}