using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Restrictions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Charts;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class MedicalServiceDetails : HealthCareComponentBase
{
    [Parameter] public Guid Id { get; set; }
    private SfAccumulationChart AccumulationChart { get; set; }
    private List<KeyValuePair<string, EnumGender>> GendersCollection { get; set; }
    private SfGrid<RestrictionDto> RestrictionGrid { get; set; }
    private string[] ToolbarItems { get; set; }
    private GetMedicalServiceInput DoctorFilter { get; set; }
    private GetMedicalServiceInput DoctorDepartmentFilter { get; set; }
    private GetRestrictionsInput RestrictionFilter { get; set; }
    private RestrictionExcelDownloadDto RestrictionFilterExcel { get; set; }
    private IReadOnlyList<DoctorDto> DoctorCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DoctorDepartmentCollection { get; set; }
    private IReadOnlyList<RestrictionDto> RestrictionCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDateCollection { get; set; }
    private IReadOnlyList<GroupedAppointmentCountDto> AppointmentByDepartmentCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private SfDialog CreateDialog { get; set; }
    private SfDialog EditDialog { get; set; }
    private bool CanDeleteRestriction { get; set; }
    private Query FilterQuery { get; set; }
    private GetServiceByDepartmentInput DepartmentFilter { get; set; }
    private LookupRequestDto RequestDto { get; set; }
    private MedicalServiceDto MedicalService { get; set; }
    private Guid EditingRestrictionId { get; set; }

    private string CurrentSorting { get; set; } = string.Empty;
    private int PageSize { get; } = 12;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private bool IsAddModalVisible { get; set; }
    private bool IsEditModalVisible { get; set; }
    private RestrictionCreateDto NewRestriction { get; set; }

    private RestrictionUpdateDto UpdateRestriction { get; set; }

    public MedicalServiceDetails()
    {
        CreateDialog = new SfDialog();
        EditDialog = new SfDialog();
        ToolbarItems = ["Add", "Delete", "Edit", "PdfExport", "ExcelExport"];
        FilterQuery = new Query();
        AccumulationChart = new SfAccumulationChart();
        RestrictionGrid = new SfGrid<RestrictionDto>();
        NewRestriction = new RestrictionCreateDto();
        UpdateRestriction = new RestrictionUpdateDto();
        EditingRestrictionId = Guid.Empty;

        RestrictionFilter = new GetRestrictionsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        RestrictionFilterExcel = new RestrictionExcelDownloadDto
        {
            MedicalServiceId = Id,
            MaxResultCount = PageSize,
            SkipCount = 0
        };

        MedicalService = new MedicalServiceDto();

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
        AppointmentByDateCollection = [];
        AppointmentByDepartmentCollection = [];
        GendersCollection = [];
        DoctorDepartmentCollection = [];
        IsAddModalVisible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
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

    private async Task SetPermissionsAsync()
    {
        CanDeleteRestriction = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.MedicalServices.Delete);
    }

    private async Task OnDepartmentChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        try
        {
            NewRestriction.DepartmentId = args.ItemData.Id;
            await GetServicesAsync(departmentId: NewRestriction.DepartmentId);
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task OnServiceChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        try
        {
            NewRestriction.MedicalServiceId = args.ItemData.Id;
            await GetDepartmentsDoctorsAsync(departmentId: NewRestriction.DepartmentId,
                medicalServiceId: NewRestriction.MedicalServiceId);
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    #region API Calls

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
        try
        {
            MedicalService = await MedicalServicesAppService.GetAsync(Id);
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetServicesAsync(Guid departmentId)
    {
        try
        {
            DepartmentFilter.DepartmentId = departmentId;

            var result =
                (await MedicalServicesAppService.GetMedicalServiceByDepartmentIdAsync(DepartmentFilter))
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
            await GetRestrictionsAsync();
            await Refresh();
        }
    }

    private async Task UpdateRestrictionAsync()
    {
        try
        {
            if (EditingRestrictionId == Guid.Empty)
            {
                return;
            }

            await RestrictionAppService.UpdateAsync(EditingRestrictionId, UpdateRestriction);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(ex.Message);
        }
        finally
        {
            CloseUpdateRestrictionModal();
            await GetRestrictionsAsync();
            await Refresh();
        }
    }

    private async Task DeleteRestrictionsAsync()
    {
        try
        {
            var selectedRecord = RestrictionGrid.GetSelectedRecordsAsync().Result;

            if (selectedRecord == null || selectedRecord.Count == 0)
            {
                return;
            }

            var ids = selectedRecord.Select(x => x.Id).ToList();

            var confirmed = await UiMessageService.Confirm(@L["DeleteSelectedRecords", ids.Count]);
            if (!confirmed)
            {
                return;
            }

            await RestrictionAppService.DeleteByIdsAsync(ids);
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
        finally
        {
            await Refresh();
        }
    }

    private async Task GetDepartmentsDoctorsAsync(Guid departmentId, Guid medicalServiceId)
    {
        try
        {
            DoctorDepartmentFilter.MaxResultCount = PageSize;
            DoctorDepartmentFilter.SkipCount = (CurrentPage - 1) * PageSize;
            DoctorDepartmentFilter.Sorting = CurrentSorting;
            DoctorDepartmentFilter.MedicalServiceId = medicalServiceId;
            DoctorDepartmentFilter.DepartmentId = departmentId;

            var doctors =
                (await MedicalServicesAppService.GetMedicalServiceWithDoctorsAsync(DoctorDepartmentFilter))
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
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetRestrictionAsync()
    {
        try
        {
            var selectedRecord = RestrictionGrid.GetSelectedRecordsAsync().Result;

            if (selectedRecord == null || selectedRecord.Count == 0)
            {
                return;
            }

            EditingRestrictionId = selectedRecord.First().Id;

            UpdateRestriction =
                ObjectMapper.Map<RestrictionDto, RestrictionUpdateDto>(
                    await RestrictionAppService.GetAsync(EditingRestrictionId));

            IsEditModalVisible = true;
        }
        catch (Exception e)
        {
            UpdateRestriction = new RestrictionUpdateDto();
            EditingRestrictionId = Guid.Empty;
            IsEditModalVisible = false;
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task SetupUpdateRestriction()
    {
        await GetRestrictionAsync();
        await MapValues();
    }

    private async Task MapValues()
    {
        await GetServicesAsync(departmentId: UpdateRestriction.DepartmentId);
        if (EditingRestrictionId == Guid.Empty)
        {
            return;
        }

        await GetDepartmentsDoctorsAsync(departmentId: UpdateRestriction.DepartmentId,
            medicalServiceId: UpdateRestriction.MedicalServiceId);
    }

    #endregion

    #region GridHandlers

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            switch (args.Item.Text)
            {
                case "Add":
                {
                    args.Cancel = true;
                    IsAddModalVisible = true;
                    break;
                }
                case "Edit":
                {
                    args.Cancel = true;
                    await SetupUpdateRestriction();
                    await GetRestrictionAsync();
                    break;
                }
                case "Delete":
                {
                    args.Cancel = true;
                    await DeleteRestrictionsAsync();
                    break;
                }
                case "Excel Export":
                {
                    args.Cancel = true;
                    await DownloadAsExcelAsync();
                    break;
                }
                case "PDF Export":
                {
                    var exportProperties = new PdfExportProperties
                    {
                        IncludeTemplateColumn = true,
                    };
                    await RestrictionGrid.ExportToPdfAsync(exportProperties);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    public void RowSelectHandler(RowSelectEventArgs<RestrictionDto> args)
    {
        var selectedRecordCount = RestrictionGrid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount > 0)
        {
            RestrictionGrid.EnableToolbarItemsAsync(["Delete"], true);
        }

        IsAddModalVisible = false;
        IsEditModalVisible = false;
    }

    private async Task Refresh()
    {
        await RestrictionGrid.Refresh();
    }

    #endregion

    private void CloseCreateRestrictionModal()
    {
        NewRestriction = new RestrictionCreateDto();
        IsAddModalVisible = false;
        CreateDialog = new SfDialog();
    }

    private void CloseUpdateRestrictionModal()
    {
        UpdateRestriction = new RestrictionUpdateDto();
        EditingRestrictionId = Guid.Empty;
        IsEditModalVisible = false;
        EditDialog = new SfDialog();
    }

    private void SetGenders()
    {
        GendersCollection = Enum.GetValues(typeof(EnumGender))
            .Cast<EnumGender>()
            .Select(e => new KeyValuePair<string, EnumGender>(e.ToString(), e))
            .ToList();
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await RestrictionAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");

        RestrictionFilterExcel.MedicalServiceId = Id;
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/restrictions/as-excel-file?DownloadToken={token}" +
            $"&MedicalServiceId={HttpUtility.UrlEncode(RestrictionFilterExcel?.MedicalServiceId?.ToString())}",
            forceLoad: true);
    }
}