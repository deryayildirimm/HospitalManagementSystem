using System.Collections.Generic;
using Pusula.Training.HealthCare.Doctors;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceWithDoctorsDto
{
    public MedicalServiceDto MedicalService { get; set; } = null!;
    public ICollection<DoctorDto> Doctors { get; set; } = null!;
}