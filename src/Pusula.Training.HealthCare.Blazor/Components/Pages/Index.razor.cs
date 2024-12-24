namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Index
{
    
    private void NavigateToDoctor()
    {
        NavigationManager.NavigateTo("/doctor/my-patients");
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
    
    private void NavigateToLaboratory()
    {
        NavigationManager.NavigateTo("/LaboratoryTechnician");
    }

    private void NavigateToTestApprovalPanel()
    {
        NavigationManager.NavigateTo("/TestApprovalPanel");
    }
}
