using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentWithNavigationProperties
{
    public Appointment Appointment { get; set; } = null!;
    public AppointmentType AppointmentType { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public MedicalService MedicalService { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
}