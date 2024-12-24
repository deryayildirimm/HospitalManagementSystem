using Microsoft.AspNetCore.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Grids
{
    public class GridColumnDefinition
    {
        public string? Field { get; set; }
        public string? HeaderText { get; set; }
        public string? Width { get; set; } = "250px";
        public Syncfusion.Blazor.Grids.TextAlign TextAlign { get; set; } = Syncfusion.Blazor.Grids.TextAlign.Justify;
        public RenderFragment? Template { get; set; }
    }
}
