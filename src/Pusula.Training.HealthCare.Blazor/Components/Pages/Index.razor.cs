namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Index
{
    
    private void NavigateToDoctor()
    {
        NavigationManager.NavigateTo("/appointments/reports/doctor-appointments");
    }

}
