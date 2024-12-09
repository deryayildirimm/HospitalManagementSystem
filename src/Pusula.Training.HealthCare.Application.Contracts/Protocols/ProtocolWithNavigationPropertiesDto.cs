using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithNavigationPropertiesDto
{
    public ProtocolDto Protocol { get; set; } = null!;
    public PatientDto Patient { get; set; } = null!;
    public DepartmentDto Department { get; set; } = null!;
    public ProtocolTypeDto ProtocolType { get; set; } = null!;
    public DoctorDto Doctor { get; set; } = null!;
    
    public InsuranceDto Insurance { get; set; } = null!;
}