using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;



namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class ProtocolTypes
{

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<ProtocolTypeDto> ProtocolTypeList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private bool CanCreateProtocolType { get; set; }
    private bool CanEditProtocolType { get; set; }
    private bool CanDeleteProtocolType { get; set; }
    private ProtocolTypeCreateDto NewProtocolTypes { get; set; }
    private Validations NewProtocolTypesValidations { get; set; } = new();
    private ProtocolTypeUpdateDto EditingProtocolType { get; set; }
    private Validations EditingProtocolTypeValidations { get; set; } = new();
    private Guid EditingProtocolTypeId { get; set; }
    private Modal CreateProtocolTypeModal { get; set; } = new();
    private Modal EditProtocolTypeModal { get; set; } = new();
    private GetProtocolTypeInput Filter { get; set; }
    private DataGridEntityActionsColumn<ProtocolTypeDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "type-create-tab";
    protected string SelectedEditTab = "type-edit-tab";



    private List<ProtocolTypeDto> SelectedProtocolTypes { get; set; } = [];
    private bool AllProtocolTypesSelected { get; set; }

    public ProtocolTypes()
    {
        NewProtocolTypes = new ProtocolTypeCreateDto();
        EditingProtocolType = new ProtocolTypeUpdateDto();
        Filter = new GetProtocolTypeInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProtocolTypeList = [];


    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();

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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["ProtocolTypes"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        Toolbar.AddButton(L["NewProtocolType"], OpenCreateProtocolTypeModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.ProtocolTypes.Create);

        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProtocolType = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.ProtocolTypes.Create);
        CanEditProtocolType = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.ProtocolTypes.Edit);
        CanDeleteProtocolType = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.ProtocolTypes.Delete);
        
    }

    private async Task GetProtocolTypesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await ProtocolTypesAppService.GetListAsync(Filter);
        ProtocolTypeList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProtocolTypesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProtocolTypesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocol-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProtocolTypeDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetProtocolTypesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProtocolTypeModalAsync()
    {
        NewProtocolTypes = new ProtocolTypeCreateDto
        {


        };

        SelectedCreateTab = "type-create-tab";


        await NewProtocolTypesValidations.ClearAll();
        await CreateProtocolTypeModal.Show();
    }

    private async Task CloseCreateProtocolTypeModalAsync()
    {
        NewProtocolTypes = new ProtocolTypeCreateDto
        {


        };
        await CreateProtocolTypeModal.Hide();
    }

    private async Task OpenEditProtocolTypeModalAsync(ProtocolTypeDto input)
    {
        SelectedEditTab = "type-edit-tab";


        var type = await ProtocolTypesAppService.GetAsync(input.Id);

        EditingProtocolTypeId = type.Id;
        EditingProtocolType = ObjectMapper.Map<ProtocolTypeDto, ProtocolTypeUpdateDto>(type);

        await EditingProtocolTypeValidations.ClearAll();
        await EditProtocolTypeModal.Show();
    }

    private async Task DeleteProtocolTypeAsync(ProtocolTypeDto input)
    {
        await ProtocolTypesAppService.DeleteAsync(input.Id);
        await GetProtocolTypesAsync();
    }

    private async Task CreateProtocolTypeAsync()
    {
        try
        {
            if (await NewProtocolTypesValidations.ValidateAll() == false)
            {
                return;
            }

            await ProtocolTypesAppService.CreateAsync(NewProtocolTypes);
            await GetProtocolTypesAsync();
            await CloseCreateProtocolTypeModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseEditProtocolTypeModalAsync()
    {
        await EditProtocolTypeModal.Hide();
    }

    private async Task UpdateProtocolTypeAsync()
    {
        try
        {
            if (await EditingProtocolTypeValidations.ValidateAll() == false)
            {
                return;
            }

            await ProtocolTypesAppService.UpdateAsync(EditingProtocolTypeId, EditingProtocolType);
            await GetProtocolTypesAsync();
            await EditProtocolTypeModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void OnSelectedCreateTabChanged(string name)
    {
        SelectedCreateTab = name;
    }

    private void OnSelectedEditTabChanged(string name)
    {
        SelectedEditTab = name;
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }


    private Task SelectAllItems()
    {
        AllProtocolTypesSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProtocolTypesSelected = false;
        SelectedProtocolTypes.Clear();

        return Task.CompletedTask;
    }

    private Task SelectedProtocolTypeRowsChanged()
    {
        if (SelectedProtocolTypes.Count != PageSize)
        {
            AllProtocolTypesSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProtocolTypeAsync()
    {
        var message = AllProtocolTypesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProtocolTypes.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProtocolTypesSelected)
        {
            await ProtocolTypesAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProtocolTypesAppService.DeleteByIdsAsync(SelectedProtocolTypes.Select(x => x.Id).ToList());
        }

        SelectedProtocolTypes.Clear();
        AllProtocolTypesSelected = false;

        await GetProtocolTypesAsync();
    }


}
