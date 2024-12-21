namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Index
{
    
    private void NavigateToDoctor()
    {
        NavigationManager.NavigateTo("/appointments/reports/doctor-appointments");
    }

    private void NavigateToAppointments()
    {
        NavigationManager.NavigateTo("/appointments/operations/create");
    }
    
    private void NavigateToPatientModule()
    {
        NavigationManager.NavigateTo("/patients");
    }
    
    private void NavigateToTreatment()
    {
        NavigationManager.NavigateTo("/my-protocols");
    }
}
