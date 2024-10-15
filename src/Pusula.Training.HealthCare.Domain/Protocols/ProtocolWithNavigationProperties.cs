using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithNavigationProperties
{
    public Protocol Protocol { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Department Department { get; set; } = null!;
}