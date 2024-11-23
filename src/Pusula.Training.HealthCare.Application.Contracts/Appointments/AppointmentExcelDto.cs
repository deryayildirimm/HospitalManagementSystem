using System;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentExcelDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public int PatientNumber { get; set; }
    public string AppointmentDate { get; set; } = string.Empty;
}