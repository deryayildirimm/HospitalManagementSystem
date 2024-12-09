using System.Collections.Generic;
using Pusula.Training.HealthCare.MedicalServices;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithMedicalService
{
    public Protocol Protocol { get; set; } = null!;
    public ICollection<MedicalService> MedicalService { get; set; } = null!;
}