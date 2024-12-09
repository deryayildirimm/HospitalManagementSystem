using System.Collections.Generic;
using Pusula.Training.HealthCare.MedicalServices;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithDetails
{
    public Protocol Protocol { get; set; } = null!;
    public List<MedicalService> MedicalService { get; set; } =  new List<MedicalService>();
}