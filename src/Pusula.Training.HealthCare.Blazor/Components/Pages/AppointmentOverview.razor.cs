using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentOverview : HealthCareComponentBase
{
    private SfAccumulationChart AccumulationChart { get; set; }
    private SfGrid<GroupedAppointmentCountDto> Grid { get; set; }
    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentsInput FilterDepartmentChart { get; set; }
    private GetAppointmentsInput FilterAppointmentByStatusChart { get; set; }
    private GetAppointmentsInput FilterRevenueByService { get; set; }
    private GetAppointmentsInput FilterAppointmentByGender { get; set; }
    private GetAppointmentsInput FilterRevenueByDepartment { get; set; }
    private GetAppointmentsInput FilterAppointmentByType { get; set; }
    private IReadOnlyList<string> GroupByOptions { get; set; }
    private GetAppointmentsInput FilterDateChart { get; set; }
    public DateTime? ChartMinDate {get;set;} = DateTime.Now;
    public DateTime? ChartMaxDate {get;set;} = DateTime.Now;
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByStatusCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByGenderCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByTypeCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> RevenuByServiceCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> RevenueByDepartmentCollection { get; set; }
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
        GroupByOptions = ["Department", "Date", "Doctor", "Service"];
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
            GroupByField = AppointmentConsts.DefaultGroupBy,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterDateChart = new GetAppointmentsInput
        {
            GroupByField = "Date",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterAppointmentByStatusChart = new GetAppointmentsInput
        {
            GroupByField = "Status",
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        
        FilterAppointmentByType = new GetAppointmentsInput
        {
            GroupByField = "AppointmentType",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterRevenueByService = new GetAppointmentsInput
        {
            GroupByField = "RevenueByService",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterRevenueByDepartment = new GetAppointmentsInput
        {
            GroupByField = "RevenueByDepartment",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterAppointmentByGender = new GetAppointmentsInput
        {
            GroupByField = "PatientGender",
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
        AppointmentByStatusCollection = [];
        RevenuByServiceCollection = [];
        AppointmentByGenderCollection = [];
        RevenueByDepartmentCollection = [];
        AppointmentByTypeCollection = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
        await SetLookupsAsync();
        await SetDatas();
        await GetGraphStatistics();
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

    #region GraphApiCalls

    private async Task GetGraphStatistics()
    {
        FilterAppointmentByStatusChart.GroupByField = "Status";
        FilterAppointmentByStatusChart.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByStatusChart.AppointmentMaxDate = ChartMaxDate;
        AppointmentByStatusCollection = await GetAppointmentsStatistics(FilterAppointmentByStatusChart);
        
        FilterRevenueByService.GroupByField = "RevenueByService";
        FilterRevenueByService.AppointmentMinDate = ChartMinDate;
        FilterRevenueByService.AppointmentMaxDate = ChartMaxDate;
        RevenuByServiceCollection = await GetAppointmentsStatistics(FilterRevenueByService);
        
        FilterAppointmentByGender.GroupByField = "PatientGender";
        FilterAppointmentByGender.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByGender.AppointmentMaxDate = ChartMaxDate;
        AppointmentByGenderCollection = await GetAppointmentsStatistics(FilterAppointmentByGender);
        
        FilterRevenueByDepartment.GroupByField = "RevenueByDepartment";
        FilterRevenueByDepartment.AppointmentMinDate = ChartMinDate;
        FilterRevenueByDepartment.AppointmentMaxDate = ChartMaxDate;
        RevenueByDepartmentCollection = await GetAppointmentsStatistics(FilterRevenueByDepartment);
        
        FilterAppointmentByType.GroupByField = "AppointmentType";
        FilterAppointmentByType.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByType.AppointmentMaxDate = ChartMaxDate;
        AppointmentByTypeCollection = await GetAppointmentsStatistics(FilterAppointmentByType);
        
    }

    #endregion
    
    private void OnChange(RangePickerEventArgs<DateTime?> args)
    {
        ChartMinDate = args.StartDate;
        ChartMaxDate = args.EndDate;
        StateHasChanged();
    }
    
}