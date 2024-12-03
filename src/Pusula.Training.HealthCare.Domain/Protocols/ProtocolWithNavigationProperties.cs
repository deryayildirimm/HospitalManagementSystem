using System.Net.Sockets;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using ProtocolType = Pusula.Training.HealthCare.ProtocolTypes.ProtocolType;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithNavigationProperties
{
    public Protocol Protocol { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Department Department { get; set; } = null!;

    public ProtocolType ProtocolType { get; set; } = null!;
}