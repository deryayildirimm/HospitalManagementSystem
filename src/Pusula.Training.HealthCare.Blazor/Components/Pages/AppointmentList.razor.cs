using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentList : HealthCareComponentBase
{
    private Query FilterQuery { get; set; }
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<AppointmentDto> Grid { get; set; }
    private int PageSize { get; } = 5;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateType { get; set; }
    private bool CanEditType { get; set; }
    private bool CanDeleteType { get; set; }
    private bool IsEditDialogVisible { get; set; }
    private bool IsCreateDialogVisible { get; set; }
    private GetAppointmentsInput Filter { get; set; }
    private AppointmentTypeCreateDto NewType { get; set; }
    private bool IsDeleteDialogVisible { get; set; }
    private SfDialog DeleteConfirmDialog { get; set; }
    private bool Flag { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private List<KeyValuePair<string, EnumPatientTypes>> PatientTypeCollection { get; set; }
    private List<KeyValuePair<string, EnumAppointmentStatus>> StatusCollection { get; set; }

    private AppointmentUpdateDto EditingAppointment { get; set; }
    private Guid EditingAppointmentId { get; set; } = default;

    public AppointmentList()
    {
        Grid = new SfGrid<AppointmentDto>();
        DeleteConfirmDialog = new SfDialog();
        NewType = new AppointmentTypeCreateDto();
        Filter = new GetAppointmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        IsEditDialogVisible = false;
        IsCreateDialogVisible = false;
        IsDeleteDialogVisible = false;
        Flag = false;
        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
        PatientTypeCollection = [];
        MedicalServiceCollection = [];
        StatusCollection = [];
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

    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
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
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await Grid.EnableToolbarItemsAsync(["Delete"], false);
            await Refresh();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateType = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.AppointmentTypes.Create);
        CanEditType = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.AppointmentTypes.Edit);
        CanDeleteType = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.AppointmentTypes.Delete);
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

    private void SetPatientTypes()
    {
        PatientTypeCollection = Enum.GetValues(typeof(EnumPatientTypes))
            .Cast<EnumPatientTypes>()
            .Select(e => new KeyValuePair<string, EnumPatientTypes>(e.ToString(), e))
            .ToList();
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

    public void Closed()
    {
        Flag = true;
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

    public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        try
        {
            switch (args.Item.Text)
            {
                case "Delete":
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
                    break;
                }
                case "Excel Export":
                {
                    var exportProperties = new ExcelExportProperties
                    {
                        IncludeTemplateColumn = true
                    };
                    await Grid.ExportToExcelAsync(exportProperties);
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

    private void OkClick()
    {
        DeleteConfirmDialog.ShowAsync();
    }

    private void CancelClick()
    {
        DeleteConfirmDialog.HideAsync();
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await AppointmentAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/appointment/as-excel-file?DownloadToken={token}&FilterText={culture}&Name=",
            forceLoad: true);
    }

    private async Task DeleteAppointmentAsync(AppointmentDto input)
    {
        try
        {
            var confirmed = await UiMessageService.Confirm(@L["DeleteConfirmationMessage"]);
            if (!confirmed)
            {
                return;
            }

            await AppointmentAppService.DeleteAsync(input.Id);
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

    private void OpenEditDialog(AppointmentDto input)
    {
        EditingAppointment = ObjectMapper.Map<AppointmentDto, AppointmentUpdateDto>(input);
        EditingAppointmentId = input.Id;
        IsEditDialogVisible = true;
    }

    private void CloseEditAppointmentModal()
    {
        EditingAppointment = new AppointmentUpdateDto();
        EditingAppointmentId = Guid.Empty;
        IsEditDialogVisible = false;
    }

    private async Task Refresh()
    {
        await Grid.Refresh();
    }

    private static string GetStatusClass(string status)
    {
        return status switch
        {
            "InProgress" => "in-progress",
            "Scheduled" => "scheduled",
            "Completed" => "completed",
            "Cancelled" => "cancelled",
            "Missed" => "missed",
            _ => string.Empty,
        };
    }
}