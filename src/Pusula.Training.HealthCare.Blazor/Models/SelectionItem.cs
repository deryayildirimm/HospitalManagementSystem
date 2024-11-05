using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class SelectionItem
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public bool IsSelected { get; set; }
}