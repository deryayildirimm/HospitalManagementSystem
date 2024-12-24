using System;
using System.Collections.Generic;
using Pusula.Training.HealthCare.MedicalServices;

namespace Pusula.Training.HealthCare.Protocols;

public class ProtocolWithDetails
{
    public Guid Id { get; set; }

    public DateTime PublishDate { get; set; }

    public float Price { get; set; }

    public string? PatientName { get; set; }
        
    public DateTime StartTime { get; set; }
    
    public Protocol Protocol { get; set; } = null!;
    
    public string[]? MedicalService { get; set; } 
}