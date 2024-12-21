using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class SlotDropdownItem
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public DateTime Date { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string DisplayText { get; set; } = string.Empty;
}