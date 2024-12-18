using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorLeaves : HealthCareComponentBase
{
    private Query FilterQuery { get; set; }
    private EditContext CreateContext { get; set; }
    private SfGrid<DoctorLeaveDto> Grid { get; set; }
    private string[] ToolbarItems { get; set; }
    private int PageSize { get; } = 10;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private bool IsEditDialogVisible { get; set; }
    private bool IsAddDialogVisible { get; set; }
    private GetDoctorLeaveInput InputFilter { get; set; }
    private GetDoctorsInput DoctorsInput { get; set; }
    private bool CanDeleteLeave { get; set; }
    private string CurrentDoctorSorting { get; set; } = string.Empty;

    //Collections
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private IReadOnlyList<DoctorLookupDto> DoctorsCollection { get; set; }
    private IReadOnlyList<DoctorLookupDto> DoctorsAddCollection { get; set; }
    private List<KeyValuePair<string, EnumLeaveType>> LeaveTypeCollection { get; set; }
    private bool IsDoctorsEnabled { get; set; }
    private bool IsNewDoctorsEnabled { get; set; }
    private DoctorLeaveUpdateDto EditingLeave { get; set; }
    private Guid EditingLeaveId { get; set; } = default;

    private Guid NewLeaveDepartmentId { get; set; } = default;
    private DoctorLeaveCreateDto NewLeave { get; set; }

    private SfToast LeaveToast { get; set; }
    private string ToastContent { get; set; } = string.Empty;
    private string ToastTitle { get; set; }
    private string ToastCssClass { get; set; } = string.Empty;

    public DoctorLeaves()
    {
        LeaveToast = new SfToast();
        ToastTitle = "Information";
        ToolbarItems = ["Add", "Delete", "Edit", "ExcelExport"];
        Grid = new SfGrid<DoctorLeaveDto>();
        InputFilter = new GetDoctorLeaveInput();
        EditingLeave = new DoctorLeaveUpdateDto();
        NewLeave = new DoctorLeaveCreateDto();
        CreateContext = new EditContext(NewLeave);
        FilterQuery = new Query();
        IsEditDialogVisible = false;
        IsDoctorsEnabled = false;
        IsAddDialogVisible = false;
        IsNewDoctorsEnabled = false;
        LeaveTypeCollection = [];
        DepartmentsCollection = [];
        MedicalServiceCollection = [];
        DoctorsCollection = [];
        DoctorsAddCollection = [];

        DoctorsInput = new GetDoctorsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentDoctorSorting
        };
    }

    protected override async Task OnInitializedAsync()
    {
        await SetLookupsAsync();
        await SetPermissionsAsync();
        SetLeaveTypes();
    }

    private async Task SetPermissionsAsync()
    {
        CanDeleteLeave = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.DoctorLeaves.Delete);
    }

    private async Task DeleteTypeAsync()
    {
        try
        {
            var selectedRecord = Grid.GetSelectedRecordsAsync().Result;

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

            await DoctorLeaveAppService.DeleteByIdsAsync(ids);
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

    private async Task SetLookupsAsync()
    {
        try
        {
            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetLeavesAsync()
    {
        SetFilters();
        await Refresh();
    }

    private void SetLeaveTypes()
    {
        LeaveTypeCollection = Enum.GetValues(typeof(EnumLeaveType))
            .Cast<EnumLeaveType>()
            .Select(e => new KeyValuePair<string, EnumLeaveType>(e.ToString(), e))
            .ToList();
    }

    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", InputFilter);
    }

    private async Task OnAddDepartmentChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        await GetDoctorsList(args.ItemData.Id, type: "AddModal");
    }

    private async Task OnDepartmentChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        InputFilter.DepartmentId = args.ItemData.Id;
        await GetDoctorsList(departmentId: args.ItemData.Id, type: "Default");
    }

    private async Task GetDoctorsList(Guid departmentId, string type)
    {
        try
        {
            DoctorsInput.DepartmentId = departmentId;

            var doctors =
                (await DoctorsAppService.GetListAsync(DoctorsInput))
                .Items
                .ToList();

            if (type == "Default")
            {
                DoctorsCollection =
                    ObjectMapper.Map<List<DoctorWithNavigationPropertiesDto>, List<DoctorLookupDto>>(doctors);
                IsDoctorsEnabled = true;
            }

            if (type == "AddModal")
            {
                DoctorsAddCollection =
                    ObjectMapper.Map<List<DoctorWithNavigationPropertiesDto>, List<DoctorLookupDto>>(doctors);
                IsNewDoctorsEnabled = true;
            }
        }
        catch (Exception e)
        {
            if (type == "Default")
            {
                DoctorsCollection = [];
                IsDoctorsEnabled = false;
            }

            if (type == "AddModal")
            {
                DoctorsAddCollection = [];
                IsNewDoctorsEnabled = false;
            }

            await UiMessageService.Error(e.Message);
        }
    }

    private async Task CreateLeaveAsync(EditContext context)
    {
        try
        {
            var isValid = context.Validate();

            if (!isValid)
            {
                await UiMessageService.Error(@L["FillRequiredFields"]);
                return;
            }

            await DoctorLeaveAppService.CreateAsync(NewLeave);
            ToastTitle = @L["LeaveCreated"];
            ToastContent = @L["LeaveCreatedSuccess"];
            ToastCssClass = "e-toast-success";
            StateHasChanged();
            await ShowOnClick();
            CloseAddModal();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(ex.Message);
        }
        finally
        {
            await Refresh();
        }
    }

    private async Task UpdateLeaveAsync()
    {
        try
        {
            if (EditingLeaveId == Guid.Empty)
            {
                return;
            }

            await DoctorLeaveAppService.UpdateAsync(EditingLeaveId, EditingLeave);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(ex.Message);
        }
        finally
        {
            CloseEditModal();
            await Refresh();
        }
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await DoctorLeaveAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/doctor-leave/as-excel-file?DownloadToken={token}",
            forceLoad: true);
    }

    private async Task GetLeaveAsync()
    {
        try
        {
            var selectedRecord = Grid.GetSelectedRecordsAsync().Result;

            if (selectedRecord == null || selectedRecord.Count == 0)
            {
                return;
            }

            EditingLeaveId = selectedRecord.First().Id;

            EditingLeave =
                ObjectMapper.Map<DoctorLeaveDto, DoctorLeaveUpdateDto>(
                    await DoctorLeaveAppService.GetAsync(EditingLeaveId));

            IsEditDialogVisible = true;
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
            CloseEditModal();
        }
    }

    private async Task ShowOnClick()
    {
        await LeaveToast.ShowAsync();
    }

    private void CloseEditModal()
    {
        IsEditDialogVisible = false;
        EditingLeave = new DoctorLeaveUpdateDto();
        EditingLeaveId = Guid.Empty;
    }

    private void OpenAddModal()
    {
        NewLeave = new DoctorLeaveCreateDto();
        IsAddDialogVisible = true;
    }

    private void CloseAddModal()
    {
        NewLeave = new DoctorLeaveCreateDto();
        IsAddDialogVisible = false;
    }

    private async Task HideToast()
    {
        await LeaveToast.HideAsync();
    }

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            switch (args.Item.Text)
            {
                case "Add":
                {
                    args.Cancel = true;
                    OpenAddModal();
                    break;
                }
                case "Edit":
                {
                    args.Cancel = true;
                    await GetLeaveAsync();
                    break;
                }
                case "Delete":
                {
                    args.Cancel = true;
                    await DeleteTypeAsync();
                    break;
                }
                case "Excel Export":
                {
                    args.Cancel = true;
                    await DownloadAsExcelAsync();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    public void RowSelectHandler(RowSelectEventArgs<DoctorLeaveDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount > 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], true);
        }
    }

    public void RowEditingHandler(RowEditingEventArgs<DoctorLeaveDto> args)
    {
        args.Cancel = true;
    }

    private async Task ClearFilters()
    {
        InputFilter = new GetDoctorLeaveInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize
        };

        StateHasChanged();
        SetFilters();
        await Refresh();
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }

    private static string GetColorClass(string status)
    {
        return status switch
        {
            "Normal" => "orange-bg",
            "Annual" => "blue-bg",
            "Sick" => "green-bg",
            "Maternity" => "red-bg",
            "Unpaid" => "purple-bg",
            _ => string.Empty,
        };
    }
}