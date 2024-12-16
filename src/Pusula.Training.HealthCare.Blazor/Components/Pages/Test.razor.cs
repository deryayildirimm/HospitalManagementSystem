using Blazorise;
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
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Blazor.Components.Modals;
using Syncfusion.Blazor.Grids;
using Pusula.Training.HealthCare.Blazor.Components.Grids;
using Syncfusion.Blazor.Inputs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Volo.Abp;
using Syncfusion.Blazor.Data;
using Autofac.Core;
using Pusula.Training.HealthCare.Blazor.Services;
using Microsoft.Extensions.DependencyInjection;



namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Test
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

    private bool SpinnerVisible = false;


    private GetProtocolTypeInput Filter { get; set; }
    private DataGridEntityActionsColumn<ProtocolTypeDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "type-create-tab";
    protected string SelectedEditTab = "type-edit-tab";

 

    private List<ProtocolTypeDto> SelectedProtocolTypes { get; set; } = [];
    private bool AllProtocolTypesSelected { get; set; }

    public Test()
    {

        Filter = new GetProtocolTypeInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        FilterQuery = new Query();

        ProtocolTypeList = [];
       

    }

    protected override async Task OnInitializedAsync()
    {
        SpinnerVisible = true;
        try
        {

            Filter = new GetProtocolTypeInput();// Filter instance'�n� burada tan�mlay�n.
            await SetPermissionsAsync();
            SetFilters();
            
        }
        finally
        {
           SpinnerVisible = false;
        }
       
    }



    private GenericModal<ProtocolTypeCreateDto> CreateModal;
    private GenericModal<ProtocolTypeUpdateDto> EditModal;
    private ProtocolTypeCreateDto newProtocol = new ProtocolTypeCreateDto();
    private ProtocolTypeUpdateDto SelectedProtocolType = new ProtocolTypeUpdateDto();
    private GenericGrid<ProtocolTypeDto> GridRef;
    private Guid EditingProtocolTypeId { get; set; }

    private void ShowModal()
    {
        newProtocol = new ProtocolTypeCreateDto();
        CreateModal?.Show();
    }

    private async Task OpenEditModal(ProtocolTypeDto protocol)
    {

        var type = await ProtocolTypesAppService.GetAsync(protocol.Id);
        
        EditingProtocolTypeId = type.Id;
        SelectedProtocolType = ObjectMapper.Map<ProtocolTypeDto, ProtocolTypeUpdateDto>(type);

        EditModal?.Show();
    }

    private async Task UpdateProtocolTypeAsync()
    {
        try
        {
            await ProtocolTypesAppService.UpdateAsync(EditingProtocolTypeId, SelectedProtocolType);

        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while deleting the Protocol Type."]);
            throw new UserFriendlyException(ex.Message);
        }
        finally
        {
            if (GridRef != null)
            {
                await GridRef.RefreshGrid();
            }
        }
          
    }
    private async Task CreateProtocolTypeAsync(ProtocolTypeCreateDto protocol)
    {

            await ProtocolTypesAppService.CreateAsync(protocol);
             await GridRef.RefreshGrid();
       
       
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
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocol-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}", forceLoad: true);
    }

    
    private List<GridColumnDefinition> Columns = new()
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
            // Foreign Key violation hatas�
            await UiMessageService.Error(@L["This Protocol Type is associated with existing Protocols and cannot be deleted."]);

        }
        catch (Exception ex)
        {
            
            await UiMessageService.Error(@L["An error occurred while deleting the Protocol Type."]);
           
        }

        await GridRef.RefreshGrid();
    }

    private Query FilterQuery { get; set; }
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
        await GridRef.RefreshGrid();
  
    }
}
