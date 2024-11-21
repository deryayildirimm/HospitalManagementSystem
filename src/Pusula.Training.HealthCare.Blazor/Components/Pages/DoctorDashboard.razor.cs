using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorDashboard : ComponentBase
{
    private DateTime CurrentDate { get; set; }

    private List<AppointmentWithNavigationPropertiesDto> DataSource { get; set; }

    private GetAppointmentsWithNavigationPropertiesInput AppointmentsFilter { get; set; }
    
    private Guid DoctorId { get; set; }
    
    private DoctorWithNavigationPropertiesDto DoctorWithNavigation { get; set; }

    #region FilterData

    private int AppointmentPageSize { get; set; } = 20;
    private int AppointmentCurrentPage { get; set; } = 1;
    #endregion

    public DoctorDashboard()
    {
        
        DoctorId = Guid.Parse("3a166153-7c6c-ed73-e8dc-8939028e1f7e");
        DoctorWithNavigation = new DoctorWithNavigationPropertiesDto();
        CurrentDate = DateTime.Now;
        AppointmentsFilter = new GetAppointmentsWithNavigationPropertiesInput
        {
            DoctorId = DoctorId,
            MaxResultCount = AppointmentPageSize,
            SkipCount = (AppointmentCurrentPage - 1) * AppointmentPageSize,
        };
        DataSource = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await GetDoctor();
        await GetAppointments();
    }

    private async Task GetDoctor()
    {
        try
        {
            DoctorWithNavigation = await DoctorAppService.GetWithNavigationPropertiesAsync(DoctorId);
            
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }
    
    private async Task GetAppointments()
    {
        try
        {
            

            var items = (await AppointmentAppService.GetListWithNavigationPropertiesAsync(AppointmentsFilter)).Items;
            DataSource = items.ToList();
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }
}