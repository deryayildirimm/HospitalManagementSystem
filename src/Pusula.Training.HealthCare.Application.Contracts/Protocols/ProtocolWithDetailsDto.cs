using System;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithDetailsDto
{
    public Guid Id { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }

    public string? PatientName { get; set; }
        
    public DateTime StartTime { get; set; }
    
    public ProtocolDto Protocol { get; set; } = null!;
    
    public string[]? MedicalService { get; set; } 
}