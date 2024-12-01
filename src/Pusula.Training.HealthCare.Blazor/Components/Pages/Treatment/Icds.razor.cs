using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Treatment.Icds;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.OpenApi.Extensions;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Popups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using SortDirection = Blazorise.SortDirection;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Treatment;

public partial class Icds
{

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<IcdDto> IcdList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private bool AllIcdsSelected { get; set; }
    private bool CanCreateIcd { get; set; }
    private bool CanEditIcd { get; set; }
    private bool CanDeleteIcd { get; set; }
    private IcdCreateDto NewIcd { get; set; }
    private IcdUpdateDto EditingIcd { get; set; }
    private Guid EditingIcdId { get; set; }
    private SfDialog CreateIcdModal;
    private SfDialog EditIcdModal;
    private GetIcdsInput Filter { get; set; }
    private List<IcdDto> SelectedIcds { get; set; } = new();
    private bool IsVisibleCreate { get; set; }
    private bool IsVisibleEdit { get; set; }

        
    public Icds()
    {
        IsVisibleCreate = false;
        IsVisibleEdit = false;
        NewIcd = new IcdCreateDto();
        EditingIcd = new IcdUpdateDto();
        Filter = new GetIcdsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        IcdList = new List<IcdDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await SearchAsync();
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Icds"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
        Toolbar.AddButton(L["NewIcd"], OpenCreateIcdModal, IconName.Add, requiredPolicyName: HealthCarePermissions.Icds.Create);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateIcd = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Icds.Create);
        CanEditIcd = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Icds.Edit);
        CanDeleteIcd = await AuthorizationService.IsGrantedAsync(HealthCarePermissions.Icds.Delete);
    }
    public async Task PageChangingHandler(GridPageChangingEventArgs args)
    {
        CurrentPage = args.CurrentPage;
        await GetIcdsAsync();
    }

    public async Task PageChangedHandler(GridPageChangedEventArgs args)
    {
        CurrentPage = args.CurrentPage;
        await GetIcdsAsync();    

    }
    private async Task GetIcdsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await IcdsAppService.GetListAsync(Filter);
        IcdList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        Console.WriteLine($"Filter: {System.Text.Json.JsonSerializer.Serialize(Filter)}");
        CurrentPage = 1;
        await GetIcdsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await IcdsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/icds/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&CodeNumber={HttpUtility.UrlEncode(Filter.CodeNumber)}&Detail={HttpUtility.UrlEncode(Filter.Detail)}", forceLoad: true);
    }

    private void OpenEditIcdModal(IcdDto icd)
    {
        EditingIcdId = icd.Id;
        EditingIcd = new IcdUpdateDto {
            Id = icd.Id,
            CodeNumber = icd.CodeNumber,
            Detail = icd.Detail
        };
        IsVisibleEdit = true;
    }
    
    private async Task OpenCreateIcdModal()
    {
        IsVisibleCreate = true;
        NewIcd = new IcdCreateDto();
    }

    private async Task CreateIcdAsync()
    {
        try
        {
            await IcdsAppService.CreateAsync(NewIcd);
            await GetIcdsAsync();
            CloseCreateIcdModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task UpdateIcdAsync()
    {
        try
        {
            await IcdsAppService.UpdateAsync(EditingIcd);
            await GetIcdsAsync();
            CloseEditIcdModal();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void CloseCreateIcdModal()
    {
        IsVisibleCreate = false;
    }

    private void CloseEditIcdModal()
    {
        IsVisibleEdit = false;
    }


    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<IcdDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetIcdsAsync();
        await InvokeAsync(StateHasChanged);
    }
    
    private async Task DeleteIcdAsync(IcdDto input)
    {
        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.CodeNumber} {input.Detail}?");
        if (!confirmed) return;

        await IcdsAppService.DeleteAsync(input.Id);
        await GetIcdsAsync();
    }

    private Task SelectAllItems()
    {
        AllIcdsSelected = true;

        return Task.CompletedTask;
    }


    private async Task ClearSelection()
    {
        SelectedIcds.Clear();
        AllIcdsSelected = false;
    }
    
    private Task SelectedIcdRowsChanged()
    {
        if (SelectedIcds.Count != PageSize)
        {
            AllIcdsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedIcdsAsync()
    {
        var message = AllIcdsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedIcds.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllIcdsSelected)
        {
            await IcdsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await IcdsAppService.DeleteByIdsAsync(SelectedIcds.Select(x => x.Id).ToList());
        }

        SelectedIcds.Clear();
        AllIcdsSelected = false;

        await GetIcdsAsync();
    }
}