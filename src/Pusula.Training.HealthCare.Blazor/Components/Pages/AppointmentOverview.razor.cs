using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentOverview : HealthCareComponentBase
{
    private SfAccumulationChart AccumulationChart { get; set; }
    private SfGrid<GroupedAppointmentCountDto> Grid { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentsInput FilterDepartmentChart { get; set; }
    private IReadOnlyList<string> GroupByOptions { get; set; }
    private GetAppointmentsInput FilterDateChart { get; set; }
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private Query FilterQuery { get; set; }

    private string CurrentSorting { get; set; } = string.Empty;
    private int PageSize { get; } = 5;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private string GroupByField { get; set; }
    

    public AppointmentOverview()
    {
        GroupByOptions = ["Department", "Date", "Doctor"];
        GroupByField = AppointmentConsts.DefaultGroupBy;
        FilterQuery = new Query();
        AccumulationChart = new SfAccumulationChart();
        Grid = new SfGrid<GroupedAppointmentCountDto>();
        Filter = new GetAppointmentsInput
        {
            GroupByField = AppointmentConsts.DefaultGroupBy,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterDepartmentChart = new GetAppointmentsInput
        {
            GroupByField =AppointmentConsts.DefaultGroupBy,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterDateChart = new GetAppointmentsInput
        {
            GroupByField = "Date",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        PatientTypeCollection = [];
        MedicalServiceCollection = [];
        StatusCollection = [];
        AppointmentByDateCollection = [];
        AppointmentByDepartmentCollection = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
        await SetLookupsAsync();
        await SetDatas();
        SetPatientTypes();
        SetStatus();
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

    private void SetStatus()
    {
        StatusCollection = Enum.GetValues(typeof(EnumAppointmentStatus))
            .Cast<EnumAppointmentStatus>()
            .Select(e => new KeyValuePair<string, EnumAppointmentStatus>(e.ToString(), e))
            .ToList();
    }

    private async Task GetAppointmentsAsync()
    {
        SetFilters();
        MapFilters();
        await SetDatas();
        await Refresh();
    }

    private async Task SetDatas()
    {
        FilterDateChart.GroupByField = "Department";
        AppointmentByDepartmentCollection = await GetAppointmentsStatistics(FilterDepartmentChart);
        
        FilterDateChart.GroupByField = "Date";
        AppointmentByDateCollection = await GetAppointmentsStatistics(FilterDateChart);
    }

    private async Task<List<GroupedAppointmentCountDto>> GetAppointmentsStatistics(GetAppointmentsInput filter)
    {
        try
        {
            return (await AppointmentAppService.GetCountByGroupAsync(filter))
                .Items
                .ToList();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
            return [];
        }
    }

    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
    }
    
    private void MapFilters()
    {
        var jsonFilter = JsonSerializer.Serialize(Filter);
        FilterDateChart = JsonSerializer.Deserialize<GetAppointmentsInput>(jsonFilter)!;
        FilterDepartmentChart = JsonSerializer.Deserialize<GetAppointmentsInput>(jsonFilter)!;
    }

    private async Task ClearFilters()
    {
        Filter = new GetAppointmentsInput
        {
            GroupByField = AppointmentConsts.DefaultGroupBy,
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

    private void OnResize(AccumulationResizeEventArgs arg)
    {
        AccumulationChart.Refresh();
    }
}