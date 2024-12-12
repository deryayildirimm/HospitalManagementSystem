using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Restrictions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MedicalServiceDetails : HealthCareComponentBase
{
    [Parameter] public Guid Id { get; set; }

    private SfAccumulationChart AccumulationChart { get; set; }
    private List<KeyValuePair<string, EnumGender>> GendersCollection { get; set; }
    private SfGrid<GroupedAppointmentCountDto> Grid { get; set; }
    private List<Object> Toolbaritems = new List<Object>() { "Add", "Delete", "Edit", "Update", "Cancel", new ItemModel() { Text = "Click", TooltipText = "Click", PrefixIcon = "e-click", Id = "Click" } };
    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentsInput FilterDepartmentChart { get; set; }
    public string[] ToolbarItems { get; set; }
    private GetAppointmentsInput FilterDateChart { get; set; }
    private GetMedicalServiceInput DoctorFilter { get; set; }
    private GetRestrictionsInput RestrictionFilter { get; set; }
    private IReadOnlyList<DoctorDto> DoctorCollection { get; set; }
    private IReadOnlyList<RestrictionDto> RestrictionCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private Query FilterQuery { get; set; }

    private LookupRequestDto RequestDto { get; set; }
    private MedicalServiceDto MedicalService { get; set; }

    private string CurrentSorting { get; set; } = string.Empty;
    private int PageSize { get; } = 12;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private bool IsAddModalVisible { get; set; }
    private bool IsEditModalVisible { get; set; }
    private RestrictionCreateDto NewRestriction { get; set; }
    
    public MedicalServiceDetails()
    {
        FilterQuery = new Query();
        AccumulationChart = new SfAccumulationChart();
        Grid = new SfGrid<GroupedAppointmentCountDto>();
        NewRestriction = new RestrictionCreateDto();
        Filter = new GetAppointmentsInput
        {
            GroupByField = AppointmentConsts.DefaultGroupBy,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        RestrictionFilter = new GetRestrictionsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        MedicalService = new MedicalServiceDto();

        FilterDepartmentChart = new GetAppointmentsInput
        {
            GroupByField = "Department",
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        FilterDateChart = new GetAppointmentsInput
        {
            GroupByField = "Date",
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        
        DoctorFilter = new GetMedicalServiceInput()
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };

        RequestDto = new LookupRequestDto
            { MaxResultCount = LookupPageSize };

        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        DoctorCollection = [];
        RestrictionCollection = [];
        MedicalServiceCollection = [];
        StatusCollection = [];
        AppointmentByDateCollection = [];
        AppointmentByDepartmentCollection = [];
        GendersCollection = [];
        IsAddModalVisible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
        await SetLookupsAsync();
        await SetDatas();
        await GetDoctorsAsync();
        await GetMedicalServiceAsync();
        await GetRestrictionsAsync();
        SetStatus();
        SetGenders();
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
                (await LookupAppService.GetAppointmentTypeLookupAsync(RequestDto))
                .Items;

            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(RequestDto))
                .Items;

            MedicalServiceCollection = (await LookupAppService.GetMedicalServiceLookupAsync(RequestDto))
                .Items;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    private void SetStatus()
    {
        StatusCollection = Enum.GetValues(typeof(EnumAppointmentStatus))
            .Cast<EnumAppointmentStatus>()
            .Select(e => new KeyValuePair<string, EnumAppointmentStatus>(e.ToString(), e))
            .ToList();
    }
    
    private async Task GetDoctorsAsync()
    {
        try
        {
            DoctorFilter.MaxResultCount = PageSize;
            DoctorFilter.SkipCount = (CurrentPage - 1) * PageSize;
            DoctorFilter.Sorting = CurrentSorting;
            DoctorFilter.MedicalServiceId = Id;

            var res = (await MedicalServicesAppService.GetMedicalServiceWithDoctorsAsync(DoctorFilter));
            DoctorCollection = res.Doctors.ToList();
        }
        catch (Exception e)
        {
            DoctorCollection = [];
            await UiMessageService.Error(e.Message);
        }
       
    }
    
    private async Task GetRestrictionsAsync()
    {
        try
        {
            RestrictionFilter.MedicalServiceId = Id;
            RestrictionCollection = 
                (await RestrictionAppService.GetListAsync(RestrictionFilter))
                .Items
                .ToList();
        }
        catch (Exception e)
        {
            RestrictionCollection = [];
            await UiMessageService.Error(e.Message);
        }
       
    }

    private async Task GetMedicalServiceAsync()
    {
        MedicalService = await MedicalServicesAppService.GetAsync(Id);
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

    public Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        switch (args.Item.Text)
        {
            case "Add":
                args.Cancel = true;
                IsAddModalVisible = true;
                return Task.CompletedTask;
            case "Edit":
                args.Cancel = true;
                IsEditModalVisible = true;
                break;
        }

        return Task.CompletedTask;
    }
    
    private void CloseCreateRestrictionModal()
    {
        NewRestriction = new RestrictionCreateDto();
        IsAddModalVisible = false;
    }
    
    private void SetGenders()
    {
        GendersCollection = Enum.GetValues(typeof(EnumGender))
            .Cast<EnumGender>()
            .Select(e => new KeyValuePair<string, EnumGender>(e.ToString(), e))
            .ToList();
    }
    
    private async Task CreateRestrictionAsync()
    {
        try
        {
            await RestrictionAppService.CreateAsync(NewRestriction);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            CloseCreateRestrictionModal();
            await Refresh();
        }
    }
}