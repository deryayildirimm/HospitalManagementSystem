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
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MedicalServiceDetails : HealthCareComponentBase
{
    [Parameter] public Guid Id { get; set; }

    private SfAccumulationChart AccumulationChart { get; set; }
    private List<KeyValuePair<string, EnumGender>> GendersCollection { get; set; }
    private SfGrid<RestrictionDto> RestrictionGrid { get; set; }

    private readonly List<Object> Toolbaritems = new List<Object>()
    {
        "Add", "Delete", "Edit", "Update", "Cancel",
        new ItemModel() { Text = "Click", TooltipText = "Click", PrefixIcon = "e-click", Id = "Click" }
    };

    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentsInput FilterDepartmentChart { get; set; }
    public string[] ToolbarItems { get; set; }
    private GetMedicalServiceInput DoctorFilter { get; set; }
    private GetMedicalServiceInput DoctorDepartmentFilter { get; set; }
    private GetRestrictionsInput RestrictionFilter { get; set; }
    private IReadOnlyList<DoctorDto> DoctorCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DoctorDepartmentCollection { get; set; }
    private IReadOnlyList<RestrictionDto> RestrictionCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }

    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private Query FilterQuery { get; set; }

    private GetServiceByDepartmentInput DepartmentFilter { get; set; }

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
        RestrictionGrid = new SfGrid<RestrictionDto>();
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

        DoctorFilter = new GetMedicalServiceInput()
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };

        DoctorDepartmentFilter = new GetMedicalServiceInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };

        DepartmentFilter = new GetServiceByDepartmentInput
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
        DoctorDepartmentCollection = [];
        IsAddModalVisible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetToolbarItemsAsync();
        await SetLookupsAsync();
        await GetDoctorsAsync();
        await GetMedicalServiceAsync();
        await GetRestrictionsAsync();
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
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    private async Task GetDoctorsAsync()
    {
        try
        {
            DoctorFilter.MaxResultCount = PageSize;
            DoctorFilter.SkipCount = (CurrentPage - 1) * PageSize;
            DoctorFilter.Sorting = CurrentSorting;
            DoctorFilter.MedicalServiceId = Id;
            DoctorCollection = (await MedicalServicesAppService.GetMedicalServiceWithDoctorsAsync(DoctorFilter)).Doctors
                .ToList();
        }
        catch (Exception e)
        {
            DoctorCollection = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetDepartmentsDoctorsAsync()
    {
        try
        {
            DoctorDepartmentFilter.MaxResultCount = PageSize;
            DoctorDepartmentFilter.SkipCount = (CurrentPage - 1) * PageSize;
            DoctorDepartmentFilter.Sorting = CurrentSorting;
            DoctorDepartmentFilter.MedicalServiceId = NewRestriction.MedicalServiceId;
            DoctorDepartmentFilter.DepartmentId = NewRestriction.DepartmentId;

            var doctors = (await MedicalServicesAppService.GetMedicalServiceWithDoctorsAsync(DoctorDepartmentFilter))
                .Doctors
                .ToList();

            DoctorDepartmentCollection = ObjectMapper.Map<List<DoctorDto>, List<LookupDto<Guid>>>(doctors);
        }
        catch (Exception e)
        {
            DoctorDepartmentCollection = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task OnDepartmentChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        try
        {
            NewRestriction.DepartmentId = args.ItemData.Id;
            await GetServicesAsync();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetServicesAsync()
    {
        try
        {
            DepartmentFilter.DepartmentId = NewRestriction.DepartmentId;
            
            var result = (await MedicalServicesAppService.GetMedicalServiceByDepartmentIdAsync(DepartmentFilter))
                .Items
                .ToList();
            
            MedicalServiceCollection = ObjectMapper.Map<List<MedicalServiceDto>, List<LookupDto<Guid>>>(result);
        }
        catch (Exception e)
        {
            MedicalServiceCollection = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task OnServiceChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        try
        {
            NewRestriction.MedicalServiceId = args.ItemData.Id;
            await GetDepartmentsDoctorsAsync();
        }
        catch (Exception e)
        {
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

    private async Task Refresh()
    {
        await RestrictionGrid.Refresh();
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
            await UiMessageService.Error(ex.Message);
        }
        finally
        {
            CloseCreateRestrictionModal();
            await Refresh();
        }
    }
}