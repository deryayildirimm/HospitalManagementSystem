using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentList : HealthCareComponentBase
{
    public PrintMode PrintMode { get; set; } = PrintMode.CurrentPage;
    private Query FilterQuery { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<AppointmentDto> Grid { get; set; }
    private List<SlotDropdownItem> AppointmentSlots { get; set; }
    private int PageSize { get; } = 5;
    private string[] PageSizes { get; set; }
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateAppointment { get; set; }
    private bool CanEditAppointment { get; set; }
    private bool CanDeleteAppointment { get; set; }
    private bool IsEditDialogVisible { get; set; }
    private bool IsCreateDialogVisible { get; set; }
    private GetAppointmentsInput Filter { get; set; }
    private GetAppointmentSlotInput GetAppointmentSlotFilter { get; set; }
    private AppointmentTypeCreateDto NewType { get; set; }
    private bool IsDeleteDialogVisible { get; set; }
    private bool IsSlotAvailable { get; set; }
    private SfDialog DeleteConfirmDialog { get; set; }
    private bool Flag { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }
    private string[] ToolbarItems { get; set; }
    private AppointmentUpdateDto EditingAppointment { get; set; }
    private Guid EditingAppointmentId { get; set; } = default;
    private bool IsEditingAppointmentDate { get; set; }
    private SlotDropdownItem SelectedSlot { get; set; }

    public AppointmentList()
    {
        ToolbarItems = ["Add", "Delete", "Edit", "ExcelExport", "Print"];
        PageSizes = ["5", "10", "15", "20"];
        SelectedSlot = new SlotDropdownItem();
        Grid = new SfGrid<AppointmentDto>();
        DeleteConfirmDialog = new SfDialog();
        NewType = new AppointmentTypeCreateDto();
        GetAppointmentSlotFilter = new GetAppointmentSlotInput
        {
            ExcludeNotAvailable = true
        };
        Filter = new GetAppointmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        IsEditDialogVisible = false;
        IsCreateDialogVisible = false;
        IsDeleteDialogVisible = false;
        IsEditingAppointmentDate = false;
        IsSlotAvailable = false;
        Flag = false;
        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        PatientTypeCollection = [];
        MedicalServiceCollection = [];
        StatusCollection = [];
        AppointmentSlots = [];
        FilterQuery = new Query();
        EditingAppointment = new AppointmentUpdateDto();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await SetLookupsAsync();
        SetPatientTypes();
        SetFilters();
        SetStatus();
    }
    
    private void SetStatus()
    {
        StatusCollection = Enum.GetValues(typeof(EnumAppointmentStatus))
            .Cast<EnumAppointmentStatus>()
            .Select(e => new KeyValuePair<string, EnumAppointmentStatus>(e.ToString(), e))
            .ToList();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Grid.EnableToolbarItemsAsync(["Delete"], false);
            await Refresh();
            await InvokeAsync(StateHasChanged);
        }
    }


    private async Task SetPermissionsAsync()
    {
        CanCreateAppointment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Create);
        CanEditAppointment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Edit);
        CanDeleteAppointment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Delete);
    }
    
    #region GridHandlers
    
    public async void OnActionBegin(ActionEventArgs<AppointmentDto> args)
    {
        if (args.RequestType.ToString() != "Delete" || !IsDeleteDialogVisible)
        {
            return;
        }

        if (args.RequestType.ToString() == "Paging")
        {
            return;
        }

        args.Cancel = true;
        await DeleteConfirmDialog.ShowAsync();
        Flag = false;
        await Refresh();
    }
    
    public void RowSelectHandler(RowSelectEventArgs<AppointmentDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount > 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], true);
        }
    }

    public void RowDeselectHandler(RowDeselectEventArgs<AppointmentDto> args)
    {
        var selectedRecordCount = Grid.GetSelectedRecordsAsync().Result.Count;
        if (selectedRecordCount == 0)
        {
            Grid.EnableToolbarItemsAsync(["Delete"], false);
        }
    }

    public static void RowEditingHandler(RowEditingEventArgs<AppointmentDto> args)
    {
        args.Cancel = true;
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
                    NavigationManager.NavigateTo("/appointments/operations/create");
                    break;
                }
                case "Edit":
                {
                    args.Cancel = true;
                    await GetAppointmentAsync();
                    break;
                }
                case "Delete":
                {
                    args.Cancel = true;
                    await DeleteAppointmentsAsync();
                    break;
                }
                case "Excel Export":
                {
                    await DownloadAsExcelAsync();
                    break;
                }
                case "Print":
                {
                    await Grid.PrintAsync();
                    break;
                }
            }
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        finally
        {
            await Refresh();
        }
    }
    
    private async Task Refresh()
    {
        await Grid.Refresh();
    }
    
    #endregion

    #region Helpers
    
    private async Task<DateTime?> GetParsedTimeAsync(string time)
    {
        if (DateTime.TryParse(time, out var parsedTime))
        {
            return parsedTime;
        }

        await UiMessageService.Error(@L["InvalidTime"]);
        return null;
    }

    private void CloseEditAppointmentModal()
    {
        EditingAppointmentId = Guid.Empty;
        IsEditDialogVisible = false;
        IsSlotAvailable = false;
        IsEditingAppointmentDate = false;
        AppointmentSlots = [];
        EditingAppointment = new AppointmentUpdateDto();
        SelectedSlot = new SlotDropdownItem();
    }
    
    private static string GetStatusClass(string status)
    {
        return status switch
        {
            "InProgress" => "orange-bg",
            "Scheduled" => "blue-bg",
            "Completed" => "green-bg",
            "Cancelled" => "red-bg",
            "Missed" => "purple-bg",
            _ => string.Empty,
        };
    }
    
    private void OkClick()
    {
        DeleteConfirmDialog.ShowAsync();
    }

    private void CancelClick()
    {
        DeleteConfirmDialog.HideAsync();
    }
    
    public void Closed()
    {
        Flag = true;
    }
    
    private void SetPatientTypes()
    {
        PatientTypeCollection = Enum.GetValues(typeof(EnumPatientTypes))
            .Cast<EnumPatientTypes>()
            .Select(e => new KeyValuePair<string, EnumPatientTypes>(e.ToString(), e))
            .ToList();
    }

    private void ToggleAppointmentEdit()
    {
        IsEditingAppointmentDate = !IsEditingAppointmentDate;
    }
    
    #endregion
    
    #region FilterOperations
    
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
    

    #endregion

    #region OnChangeMethods

        private async Task OnEditDateChange(ChangedEventArgs<DateTime> args)
        {
            GetAppointmentSlotFilter.MedicalServiceId = EditingAppointment.MedicalServiceId;
            GetAppointmentSlotFilter.DoctorId = EditingAppointment.DoctorId;
            await GetAvailableSlots();
        }
    
        private async Task OnNewSlotChange(SelectEventArgs<SlotDropdownItem> args)
        {
            try
            {
                var itemData = args.ItemData;
                if (itemData is null)
                {
                    return;
                }
    
                var parsedStart = await GetParsedTimeAsync(itemData.StartTime);
                var parsedEnd = await GetParsedTimeAsync(itemData.EndTime);
    
                if (parsedStart is null || parsedEnd is null)
                {
                    return;
                }
    
                EditingAppointment.AppointmentDate = itemData.Date;
                EditingAppointment.StartTime =
                    itemData.Date.AddHours(parsedStart.Value.Hour).AddMinutes(parsedStart.Value.Minute);
                EditingAppointment.EndTime =
                    itemData.Date.AddHours(parsedEnd.Value.Hour).AddMinutes(parsedEnd.Value.Minute);
            }
            catch (Exception e)
            {
                await UiMessageService.Error(e.Message);
            }
        }

    #endregion
    
    #region APICalls

    private async Task GetAvailableSlots()
    {
        try
        {
            var slots =
                (await AppointmentAppService.GetAvailableSlotsAsync(GetAppointmentSlotFilter)).Items;

            if (!slots.Any())
            {
                AppointmentSlots = [];
                return;
            }

            AppointmentSlots = slots
                .Select(x => new SlotDropdownItem
                {
                    Id = Guid.NewGuid(),
                    DoctorId = x.DoctorId,
                    MedicalServiceId = x.MedicalServiceId,
                    Date = x.Date,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    DisplayText = $"{x.StartTime}-{x.EndTime}"
                })
                .ToList();

            IsSlotAvailable = AppointmentSlots.Count > 0;
        }
        catch (BusinessException e)
        {
            IsSlotAvailable = false;
            AppointmentSlots = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetAppointmentAsync()
    {
        try
        {
            var selectedRecord = Grid.GetSelectedRecordsAsync().Result;

            if (selectedRecord == null || selectedRecord.Count == 0)
            {
                return;
            }

            EditingAppointmentId = selectedRecord.First().Id;

            EditingAppointment =
                ObjectMapper.Map<AppointmentDto, AppointmentUpdateDto>(
                    await AppointmentAppService.GetAsync(EditingAppointmentId));

            IsEditDialogVisible = true;
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
            CloseEditAppointmentModal();
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
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetAppointmentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        SetFilters();
        await Refresh();
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await AppointmentAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/appointment/as-excel-file?DownloadToken={token}&FilterText={culture}&Name=",
            forceLoad: true);
    }

    private async Task DeleteAppointmentsAsync()
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

            await AppointmentAppService.DeleteByIdsAsync(ids);
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

    private async Task UpdateAppointmentAsync()
    {
        try
        {
            await AppointmentAppService.UpdateAsync(EditingAppointmentId, EditingAppointment);
            CloseEditAppointmentModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            CloseEditAppointmentModal();
            await Refresh();
        }
    }

    #endregion
    
}