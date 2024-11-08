using System;

namespace Pusula.Training.HealthCare.Blazor.Models;

public class SelectionItem
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = null!;
    public bool IsSelected { get; set; }
    
    public double Cost { get; set; }
    
    public override bool Equals(object obj)
    {
        if (obj is SelectionItem item)
        {
            return Id == item.Id; 
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}