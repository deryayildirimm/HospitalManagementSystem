using System.Collections.Generic;
using Pusula.Training.HealthCare.MedicalServices;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolsWithMedicalServiceDto
{
    public ProtocolDto Protocol { get; set; } = null!;
    public ICollection<MedicalServiceDto> MedicalService { get; set; } = null!;
}