using System;
using Pusula.Training.HealthCare.Patients;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class Doctor
{
    public Guid Id { get; set; }
    public bool IsSelected { get; set; }
    public required string Name { get; set; }
    public required string Department { get; set; }
    public required EnumGender Gender { get; set; }
    public bool IsAvailable { get; set; }
}