using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentWithNavigationPropertiesDto
{
    public AppointmentDto Appointment { get; set; } = null!;
    public AppointmentTypeDto AppointmentType { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
    public MedicalServiceDto MedicalService { get; set; } = null!;
    public PatientDto Patient { get; set; } = null!;
}