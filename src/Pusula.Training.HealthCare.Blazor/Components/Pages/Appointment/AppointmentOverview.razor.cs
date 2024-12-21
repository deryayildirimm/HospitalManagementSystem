using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;
using Volo.Abp;
using ExportType = Syncfusion.Blazor.Charts.ExportType;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Appointment;

public partial class AppointmentOverview : HealthCareComponentBase
{
    private ElementReference Element { get; set; }
    private SfAccumulationChart AccumulationChart { get; set; }
    private SfGrid<AppointmentStatisticDto> Grid { get; set; }
    private SfAccumulationChart ChartByStatus { get; set; }
    private SfAccumulationChart ChartRevenueByService { get; set; }
    private SfAccumulationChart ChartByGender { get; set; }
    private SfAccumulationChart ChartRevenueByDepartment { get; set; }
    private SfChart ChartByType { get; set; }
    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentsInput FilterDepartmentChart { get; set; }
    private GetAppointmentsInput FilterAppointmentByStatusChart { get; set; }
    private GetAppointmentsInput FilterRevenueByService { get; set; }
    private GetAppointmentsInput FilterAppointmentByGender { get; set; }
    private GetAppointmentsInput FilterRevenueByDepartment { get; set; }
    private GetAppointmentsInput FilterAppointmentByType { get; set; }
    private IReadOnlyList<EnumAppointmentGroupFilter> GroupByOptions { get; set; }
    private GetAppointmentsInput FilterDateChart { get; set; }
    private DateTime? ChartMinDate { get; set; }
    private DateTime? ChartMaxDate { get; set; }
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> AppointmentByStatusCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> AppointmentByGenderCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> AppointmentByTypeCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> RevenueByServiceCollection { get; set; }
    private IReadOnlyList<AppointmentStatisticDto> RevenueByDepartmentCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private Query FilterQuery { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private int PageSize { get; } = 5;
    private string[] PageSizes { get; set; }
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private EnumAppointmentGroupFilter GroupByField { get; set; }
    
    public AppointmentOverview()
    {
        ChartRevenueByService = new SfAccumulationChart();
        ChartByGender = new SfAccumulationChart();
        ChartRevenueByDepartment = new SfAccumulationChart();
        ChartByType = new SfChart();
        ChartMinDate = DateTime.Now.AddDays(-7);
        ChartMaxDate = DateTime.Now.AddDays(14);
        GroupByOptions =
        [
            EnumAppointmentGroupFilter.Department,
            EnumAppointmentGroupFilter.Date,
            EnumAppointmentGroupFilter.Doctor,
            EnumAppointmentGroupFilter.Service
        ];
        GroupByField = AppointmentConsts.DefaultGroupBy;
        Element = new ElementReference();
        FilterQuery = new Query();
        AccumulationChart = new SfAccumulationChart();
        Grid = new SfGrid<AppointmentStatisticDto>();
        ChartByStatus = new SfAccumulationChart();
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
            GroupByField = EnumAppointmentGroupFilter.Date,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterAppointmentByStatusChart = new GetAppointmentsInput
        {
            GroupByField = EnumAppointmentGroupFilter.Status,
            AppointmentMinDate = ChartMinDate,
            AppointmentMaxDate = ChartMaxDate,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterAppointmentByType = new GetAppointmentsInput
        {
            GroupByField = EnumAppointmentGroupFilter.AppointmentType,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterRevenueByService = new GetAppointmentsInput
        {
            GroupByField = EnumAppointmentGroupFilter.RevenueByService,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterRevenueByDepartment = new GetAppointmentsInput
        {
            GroupByField = EnumAppointmentGroupFilter.RevenueByDepartment,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterAppointmentByGender = new GetAppointmentsInput
        {
            GroupByField = EnumAppointmentGroupFilter.PatientGender,
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        PageSizes = ["5", "10", "15", "20"];
        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        PatientTypeCollection = [];
        MedicalServiceCollection = [];
        StatusCollection = [];
        AppointmentByDateCollection = [];
        AppointmentByDepartmentCollection = [];
        AppointmentByStatusCollection = [];
        RevenueByServiceCollection = [];
        AppointmentByGenderCollection = [];
        RevenueByDepartmentCollection = [];
        AppointmentByTypeCollection = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetLookupsAsync();
        await SetGroupingData();
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
        await SetGroupingData();
        await Refresh();
    }

    private async Task SetGroupingData()
    {
        FilterDateChart.GroupByField = EnumAppointmentGroupFilter.Department;
        AppointmentByDepartmentCollection = await GetAppointmentsStatistics(FilterDepartmentChart);

        FilterDateChart.GroupByField = EnumAppointmentGroupFilter.Date;
        AppointmentByDateCollection = await GetAppointmentsStatistics(FilterDateChart);
    }
    
    private async Task Refresh()
    {
        await Grid.Refresh();
    }

    private void OnResize(AccumulationResizeEventArgs arg)
    {
        AccumulationChart.Refresh();
    }
    
    #region Filters

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

    #endregion

    #region GraphApiCalls

    private async Task GetGraphStatistics()
    {
        FilterAppointmentByStatusChart.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByStatusChart.AppointmentMaxDate = ChartMaxDate;
        AppointmentByStatusCollection = await GetAppointmentsStatistics(FilterAppointmentByStatusChart);

        FilterRevenueByService.AppointmentMinDate = ChartMinDate;
        FilterRevenueByService.AppointmentMaxDate = ChartMaxDate;
        RevenueByServiceCollection = await GetAppointmentsStatistics(FilterRevenueByService);

        FilterAppointmentByGender.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByGender.AppointmentMaxDate = ChartMaxDate;
        AppointmentByGenderCollection = await GetAppointmentsStatistics(FilterAppointmentByGender);

        FilterRevenueByDepartment.AppointmentMinDate = ChartMinDate;
        FilterRevenueByDepartment.AppointmentMaxDate = ChartMaxDate;
        RevenueByDepartmentCollection = await GetAppointmentsStatistics(FilterRevenueByDepartment);

        FilterAppointmentByType.AppointmentMinDate = ChartMinDate;
        FilterAppointmentByType.AppointmentMaxDate = ChartMaxDate;
        AppointmentByTypeCollection = await GetAppointmentsStatistics(FilterAppointmentByType);
    }

    private async Task<List<AppointmentStatisticDto>> GetAppointmentsStatistics(GetAppointmentsInput filter)
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

    #endregion

    #region OnChangeMethods
    private void OnGraphicDateChange(RangePickerEventArgs<DateTime?> args)
    {
        ChartMinDate = args.StartDate;
        ChartMaxDate = args.EndDate;
        StateHasChanged();
    }

    private async Task OnGroupByChange(SelectEventArgs<EnumAppointmentGroupFilter> args)
    {
        try
        {
            Filter.GroupByField = args.ItemData;
            await GetAppointmentsAsync();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }
    
    #endregion
    
    private async Task ExportItemSelected(MenuEventArgs args)
    {
        switch (args.Item.Text)
        {
            case "PDF":
                await ExportCharts(ExportType.PDF);
                break;
            case "XLS":
                await ExportCharts(ExportType.XLSX);
                break;
            case "CSV":
                await ExportCharts(ExportType.CSV);
                break;
        }
    }

    private async Task PrintCharts()
    {
        await ChartByStatus.PrintAsync(Element);
    }

    private async Task ExportCharts(ExportType type)
    {
        await ChartByType.ExportAsync(type, ChartByType.Title, PdfPageOrientation.Landscape);
        await ChartByStatus.ExportAsync(ExportType.PNG, ChartByStatus.Title);
        await ChartRevenueByService.ExportAsync(ExportType.PNG, ChartRevenueByService.Title);
        await ChartByGender.ExportAsync(ExportType.PNG, ChartByGender.Title);
        await ChartRevenueByDepartment.ExportAsync(ExportType.PNG, ChartRevenueByDepartment.Title);
    }
}