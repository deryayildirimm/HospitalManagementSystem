using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Treatment;

public partial class MyProtocols
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<ProtocolWithNavigationPropertiesDto> ProtocolList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private GetProtocolsInput Filter { get; set; }
    private DataGridEntityActionsColumn<ProtocolWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "protocol-create-tab";
    protected string SelectedEditTab = "protocol-edit-tab";

    private IReadOnlyList<LookupDto<Guid>> PatientsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DoctorsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> ProtocolTypesCollection { get; set; } = [];
    private List<ProtocolWithNavigationPropertiesDto> SelectedProtocols { get; set; } = [];

    private bool AllProtocolsSelected { get; set; }

    public MyProtocols()
    {
        Filter = new GetProtocolsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProtocolList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await GetDepartmentCollectionLookupAsync();
        await GetDoctorCollectionLookupAsync();
        await GetProtocolTypeCollectionLookupAsync();
        await GetPatientCollectionLookupAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["MyProtocols"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        return ValueTask.CompletedTask;
    }

    private async Task GetProtocolsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await ProtocolsAppService.GetListAsync(Filter);
        ProtocolList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProtocolsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProtocolsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
   //     NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocols/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Type={HttpUtility.UrlEncode(Filter.Type)}&StartTimeMin={Filter.StartTimeMin?.ToString("O")}&StartTimeMax={Filter.StartTimeMax?.ToString("O")}&EndTime={HttpUtility.UrlEncode(Filter.EndTime)}&PatientId={Filter.PatientId}&DepartmentId={Filter.DepartmentId}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProtocolWithNavigationPropertiesDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetProtocolsAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnStartTimeMinChangedAsync(DateTime? startTimeMin)
    {
        Filter.StartTimeMin = startTimeMin.HasValue ? startTimeMin.Value.Date : startTimeMin;
        await SearchAsync();
    }
    protected virtual async Task OnStartTimeMaxChangedAsync(DateTime? startTimeMax)
    {
        Filter.StartTimeMax = startTimeMax.HasValue ? startTimeMax.Value.Date.AddDays(1).AddSeconds(-1) : startTimeMax;
        await SearchAsync();
    }
    
    protected virtual async Task OnEndTimeMinChangedAsync(DateTime? endTimeMin)
         {
             Filter.StartTimeMin = endTimeMin.HasValue ? endTimeMin.Value.Date : endTimeMin;
             await SearchAsync();
         }
    
    protected virtual async Task OnEndTimeMaxChangedAsync(DateTime? endTimeMax)
    {
        Filter.StartTimeMin = endTimeMax.HasValue ? endTimeMax.Value.Date : endTimeMax;
        await SearchAsync();
    }
    protected virtual async Task OnPatientIdChangedAsync(Guid? patientId)
    {
        Filter.PatientId = patientId;
        await SearchAsync();
    }
    protected virtual async Task OnDepartmentIdChangedAsync(Guid? departmentId)
    {
        Filter.DepartmentId = departmentId;
        await SearchAsync();
    }
    protected virtual async Task OnDoctorIdChangedAsync(Guid? doctorId)
    {
        Filter.DoctorId = doctorId;
        await SearchAsync();
    }
    
    protected virtual async Task OnProtocolTypeIdChangedAsync(Guid? protocolTypeId)
    {
        Filter.ProtocolTypeId = protocolTypeId;
        await SearchAsync();
    }
    
    private async Task GetPatientCollectionLookupAsync(string? newValue = null)
    {
        PatientsCollection = (await ProtocolsAppService.GetPatientLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
    {
        DepartmentsCollection = (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task GetDoctorCollectionLookupAsync(string? newValue = null)
    {
        DoctorsCollection = (await LookupAppService.GetDoctorLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    #region protocoltype lookup
  
    private async Task GetProtocolTypeCollectionLookupAsync(string? newValue = null)
    {
        ProtocolTypesCollection = (await LookupAppService.GetProtocolTypeLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    

    #endregion
    

    private Task SelectAllItems()
    {
        AllProtocolsSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProtocolsSelected = false;
        SelectedProtocols.Clear();

        return Task.CompletedTask;
    }

    private Task SelectedProtocolRowsChanged()
    {
        if (SelectedProtocols.Count != PageSize)
        {
            AllProtocolsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProtocolsAsync()
    {
        var message = AllProtocolsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProtocols.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProtocolsSelected)
        {
            await ProtocolsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProtocolsAppService.DeleteByIdsAsync(SelectedProtocols.Select(x => x.Protocol.Id).ToList());
        }

        SelectedProtocols.Clear();
        AllProtocolsSelected = false;

        await GetProtocolsAsync();
    }

    private void NavigateToExamination(Guid protocolId)
    {
        ProtocolStateService.ProtocolId = protocolId;
        NavigationManager.NavigateTo($"/examination?protocolId={protocolId}");
    }
}
