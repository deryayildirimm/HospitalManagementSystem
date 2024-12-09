using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentOverview : HealthCareComponentBase
{
    private SfGrid<DepartmentAppointmentCount> Grid { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private GetAppointmentsInput Filter { get; set; }
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private Query FilterQuery { get; set; }

    private string CurrentSorting { get; set; } = string.Empty;
    private int PageSize { get; } = 5;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    
    public AppointmentOverview()
    {
        FilterQuery = new Query();
        Grid = new SfGrid<DepartmentAppointmentCount>();
        Filter = new GetAppointmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        
        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        PatientTypeCollection = [];
        MedicalServiceCollection = [];
    }
    
    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
        await SetLookupsAsync();
        SetPatientTypes();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
    
    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        return ValueTask.CompletedTask;
    }
    
    private async Task SetLookupsAsync()
    {
        try
        {
            AppointmentTypesCollection =
                (await LookupAppService.GetAppointmentTypeLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

            MedicalServiceCollection = (await LookupAppService.GetMedicalServiceLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }
    
    private void SetPatientTypes()
    {
        PatientTypeCollection = Enum.GetValues(typeof(EnumPatientTypes))
            .Cast<EnumPatientTypes>()
            .Select(e => new KeyValuePair<string, EnumPatientTypes>(e.ToString(), e))
            .ToList();
    }
    
    private async Task GetAppointmentsAsync()
    {
        SetFilters();
        await Refresh();
    }
    
    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
    }
    
    private async Task ClearFilters()
    {
        Filter = new GetAppointmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }
}