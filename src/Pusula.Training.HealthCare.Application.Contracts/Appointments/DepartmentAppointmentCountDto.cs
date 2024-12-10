using System;
using Pusula.Training.HealthCare.Departments;

namespace Pusula.Training.HealthCare.Appointments;

public class DepartmentAppointmentCountDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
}