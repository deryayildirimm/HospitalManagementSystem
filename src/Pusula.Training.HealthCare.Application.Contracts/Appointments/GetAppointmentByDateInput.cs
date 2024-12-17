using System;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentByDateInput
{
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime AppointmentDate { get; set; }
}