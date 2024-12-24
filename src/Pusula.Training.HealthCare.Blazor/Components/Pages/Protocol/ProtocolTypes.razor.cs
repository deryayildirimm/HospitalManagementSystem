using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Pusula.Training.HealthCare.Blazor.Components.Modals;
using Pusula.Training.HealthCare.Blazor.Components.Grids;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Volo.Abp;
using Syncfusion.Blazor.Data;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Protocol;

public partial class ProtocolTypes : HealthCareComponentBase
{

    protected readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; set; } = new PageToolbar();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateProtocolType { get; set; }
    private bool CanEditProtocolType { get; set; }
    private bool CanDeleteProtocolType { get; set; }
    private Query FilterQuery { get; set; }

    private bool _spinnerVisible;
    private ProtocolTypeCreateDto _newProtocol;
    private ProtocolTypeUpdateDto _selectedProtocolType;
    
    private GenericGrid<ProtocolTypeDto> _gridRef;
    private GenericModal<ProtocolTypeCreateDto> _createModal;
    private GenericModal<ProtocolTypeUpdateDto> _editModal;
    private Guid EditingProtocolTypeId { get; set; }
    
    private GetProtocolTypeInput Filter { get; set; }
    
    public ProtocolTypes()
    {
        _gridRef = new GenericGrid<ProtocolTypeDto>();
        _createModal = new GenericModal<ProtocolTypeCreateDto>();
        _editModal = new GenericModal<ProtocolTypeUpdateDto>();
        _newProtocol = new ProtocolTypeCreateDto();
        _selectedProtocolType = new ProtocolTypeUpdateDto();
        Filter = new GetProtocolTypeInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        FilterQuery = new Query();
    }

    protected override async Task OnInitializedAsync()
    {
        _spinnerVisible = true;
        try
        {
            Filter = new GetProtocolTypeInput();
            await SetPermissionsAsync();
            SetFilters();
        }
        finally
        {
            _spinnerVisible = false;
        }
    }
    
    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
    }
    
    private async Task HandleFilterChanged(GetProtocolTypeInput updatedFilter)
    {

        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        SetFilters();
        await _gridRef.RefreshGrid();
  
    }
    
    private async Task ClearFilters()
    {
        Filter = new GetProtocolTypeInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };
        StateHasChanged();
        SetFilters();
        await _gridRef.RefreshGrid();
    }
    
    private  Task ShowModal()
    {
        _newProtocol = new ProtocolTypeCreateDto();
        _createModal?.Show();
        return Task.CompletedTask;
    }

    private async Task OpenEditModal(ProtocolTypeDto protocol)
    {

        var type = await ProtocolTypesAppService.GetAsync(protocol.Id);
        
        EditingProtocolTypeId = type.Id;
        _selectedProtocolType = ObjectMapper.Map<ProtocolTypeDto, ProtocolTypeUpdateDto>(type);
        _editModal?.Show();
    }

    private async Task UpdateProtocolTypeAsync()
    {
        try
        {
            await ProtocolTypesAppService.UpdateAsync(EditingProtocolTypeId, _selectedProtocolType);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while updating the Protocol Type."]);
            throw new UserFriendlyException(ex.Message);
        }
        finally
        {
                await _gridRef.RefreshGrid();
        }
          
    }
    private async Task CreateProtocolTypeAsync(ProtocolTypeCreateDto protocol)
    {
        try
        {
            await ProtocolTypesAppService.CreateAsync(protocol);
        }
        catch (Exception e)
        {
            await UiMessageService.Error(@L[$"An error occurred while creating the Protocol Type. {e?.InnerException?.Message}"]);
            throw new UserFriendlyException(e!.Message);
        }
        finally
        {
            await _gridRef.RefreshGrid();
        }
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
        Toolbar.AddButton(L["Create Protocol Type"], ShowModal, IconName.Add, requiredPolicyName: HealthCarePermissions.ProtocolTypes.Create);
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
    
    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProtocolTypesAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocol-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}"
                                     +
                                     $"{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}" 
                                  
            , forceLoad: true);
    }

    
    private readonly List<GridColumnDefinition> _columns = new()
    {
        new GridColumnDefinition { Field = "Name", HeaderText = "Name", Width = "200px" },
    };



    private async Task DeleteProtocolTypeAsync(ProtocolTypeDto input)
    {

        try
        {
            await ProtocolTypesAppService.DeleteAsync(input.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
        {
            await UiMessageService.Error(@L["This Protocol Type is associated with existing Protocols and cannot be deleted."]);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while deleting the Protocol Type."] + ex.Message);
        }
        await _gridRef.RefreshGrid();
    }
   
}
