using System;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolMedicalService : Entity
{
    public Guid ProtocolId { get; set; }
    public Protocol Protocol { get; set; }
    
    public Guid MedicalServiceId { get; set; }
    
    public MedicalService MedicalService { get; set; }
    
   
    public override object[] GetKeys()
    {
        return new object[] {ProtocolId, MedicalServiceId};
    }
    
    
}