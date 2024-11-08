using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class Doctor
{
    public Guid Id { get; set; }
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public string Department { get; set; }
    public string Gender { get; set; }
    public string? InsuranceType { get; set; }
    public bool IsAvailable { get; set; }
}