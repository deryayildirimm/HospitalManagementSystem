using System.Collections.Generic;
using Pusula.Training.HealthCare.Doctors;

namespace Pusula.Training.HealthCare.MedicalServices;

public class MedicalServiceWithDoctors
{
    public MedicalService MedicalService { get; set; } = null!;
    public ICollection<Doctor> Doctors { get; set; } = null!;
    
}