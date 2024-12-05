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
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Volo.Abp;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class AppointmentList : HealthCareComponentBase
{
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private SfGrid<AppointmentDto> Grid { get; set; }
    private int PageSize { get; } = 20;
    private int LookupPageSize { get; } = 100;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateType { get; set; }
    private bool CanEditType { get; set; }
    private bool CanDeleteType { get; set; }
    private bool IsEditDialogVisible { get; set; }
    private bool IsCreateDialogVisible { get; set; }
    private GetAppointmentTypesInput Filter { get; set; }
    private AppointmentTypeUpdateDto EditingType { get; set; }
    private Guid EditingTypeId { get; set; } = default;
    private AppointmentTypeCreateDto NewType { get; set; }
    private bool IsDeleteDialogVisible { get; set; }
    private SfDialog DeleteConfirmDialog { get; set; }
    private bool Flag { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; }
    
    public AppointmentList()
    {
        Filter = new GetAppointmentTypesInput();
        NewType = new AppointmentTypeCreateDto();
        Filter = new GetAppointmentTypesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        
        EditingType = new AppointmentTypeUpdateDto();
        IsEditDialogVisible = false;
        IsCreateDialogVisible = false;
        IsDeleteDialogVisible = false;
        Flag = false;
        AppointmentTypesCollection = [];
        DepartmentsCollection = [];
    }
    
    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        //await SetLookupsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await Grid.EnableToolbarItemsAsync(["Delete"], false);
            await Grid.Refresh();
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
        Toolbar.AddButton(L["NewAppointmentType"], OpenCreateTypeModalAsync, IconName.Plus,
            requiredPolicyName: HealthCarePermissions.AppointmentTypes.Create);

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
                (await AppointmentAppService.GetAppointmentTypeLookupAsync(new LookupRequestDto
                    {MaxResultCount = LookupPageSize }))
                .Items;        
        
            DepartmentsCollection = 
                (await ProtocolsAppService.GetDepartmentLookupAsync(new LookupRequestDto 
                    { MaxResultCount = LookupPageSize }))
                .Items;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        
    }
    
    
    public void OnActionBegin(ActionEventArgs<AppointmentDto> args)
    {
        if (args.RequestType.ToString() != "Delete" || !IsDeleteDialogVisible)
        {
            return;
        }

        args.Cancel = true;
        DeleteConfirmDialog.ShowAsync();
        Flag = false;
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
            Refresh();
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

    private void OpenEditDialog(AppointmentDto input)
    {
        //EditingType = ObjectMapper.Map<AppointmentDto, AppointmentUpdateDto>(input);
        EditingTypeId = input.Id;
        IsEditDialogVisible = true;
    }

    private void CloseEditTypeModal()
    {
        EditingType = new AppointmentTypeUpdateDto();
        EditingTypeId = Guid.Empty;
        IsEditDialogVisible = false;
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
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/appointment/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}",
            forceLoad: true);
    }
    
    private Task OpenCreateTypeModalAsync()
    {
        IsCreateDialogVisible = true;
        return Task.CompletedTask;
    }

    private void CloseCreateTypeModal()
    {
        NewType = new AppointmentTypeCreateDto();
        IsCreateDialogVisible = false;
    }

    private async Task DeleteTypeAsync(AppointmentDto input)
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
            Refresh();
        }
    }

    private async Task CreateTypeAsync()
    {
        try
        {
            await AppointmentAppService.CreateAsync(new AppointmentCreateDto());
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            CloseCreateTypeModal();
            Refresh();
        }
    }

    private async Task UpdateTypeAsync()
    {
        try
        {
            await AppointmentAppService.UpdateAsync(EditingTypeId, new AppointmentUpdateDto());
            CloseEditTypeModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            CloseEditTypeModal();
            Refresh();
        }
    }

    private void Refresh()
    {
        Grid.Refresh();
    }
}
