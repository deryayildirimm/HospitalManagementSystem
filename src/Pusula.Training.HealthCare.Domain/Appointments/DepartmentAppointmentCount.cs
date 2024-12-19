using System;

namespace Pusula.Training.HealthCare.Appointments;

public class DepartmentAppointmentCount
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}