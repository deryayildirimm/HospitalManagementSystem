using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Pusula.Training.HealthCare.Blazor.Components.Grids;
using Pusula.Training.HealthCare.Lookups;
using Syncfusion.Blazor.Data;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Protocol.Reports;

public partial class DepartmentProtocolReports : HealthCareComponentBase
{

    protected readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int LookupPageSize { get; } = 50;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateProtocolType { get; set; }
    private bool CanEditProtocolType { get; set; }
    private bool CanDeleteProtocolType { get; set; }

    private bool _spinnerVisible;
    private bool IsLookupsLoaded { get; set; } 
    private GenericGrid<DepartmentStatisticDto> _gridRefFirst;
    private GenericGrid<ProtocolPatientDepartmentListReportDto> _gridRefSec;
    private Query FirstQuery { get; set; }
    
    private Query SecQuery { get; set; }
    private GetProtocolsInput FilterFirst { get; set; }
    private GetProtocolsInput FilterSec { get; set; }
  

    public DepartmentProtocolReports()
    {
        _gridRefFirst = new GenericGrid<DepartmentStatisticDto>();
        _gridRefSec = new GenericGrid<ProtocolPatientDepartmentListReportDto>();
        FilterFirst = new GetProtocolsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        FilterSec = new GetProtocolsInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0
        };
        FirstQuery = new Query();
        SecQuery = new Query();

    }
    
    
    private void NavigateToDetail(ProtocolPatientDepartmentListReportDto patientList)
    {
       
        NavigationManager.NavigateTo($"/patients/{patientList.PatientNumber}/detail");
    }

    #region Initial part
    
   
    
    protected override async Task OnInitializedAsync()
    {
        
        _spinnerVisible = true;
        try
        {
            FilterFirst.MaxResultCount = PageSize;
            FilterFirst.SkipCount = (CurrentPage - 1) * PageSize;
            FilterFirst.Sorting = CurrentSorting;
            
            FilterSec.MaxResultCount = PageSize;
            FilterSec.SkipCount = (CurrentPage - 1) * PageSize;
            FilterSec.Sorting = CurrentSorting;
            
            FilterFirst = new GetProtocolsInput();
            FilterSec = new GetProtocolsInput();
            await SetPermissionsAsync();
            SetFiltersFirst();
            SetFiltersSec();
        }
        finally
        {
            _spinnerVisible = false;
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Protocol-Department Statistics"]));
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
            .IsGrantedAsync(HealthCarePermissions.Protocols.Create);
        CanEditProtocolType = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Protocols.Edit);
        CanDeleteProtocolType = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Protocols.Delete);
        
    }
    
    private readonly List<GridColumnDefinition> _columnsFirst = new()
    {
        new GridColumnDefinition { Field = "DepartmentName", HeaderText = "Department", Width = "200px" },
        new GridColumnDefinition { Field = "PatientCount", HeaderText = "Patient Count", Width = "150px" },
    };

    private readonly List<GridColumnDefinition> _columnsSec = new()
    {
        new GridColumnDefinition { Field = "DepartmentName", HeaderText = "Department", Width = "200px" },
        new GridColumnDefinition { Field = "FullName", HeaderText = "Full Name", Width = "200px" },
        new GridColumnDefinition { Field = "ProtocolCount", HeaderText = "Protocol Count", Width = "150px" },
        new GridColumnDefinition { Field = "LastVisit", HeaderText = "Last Visit", Width = "150px" },
    };
    
    #endregion
    
    
    private async Task LoadLookupsAsync()
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
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocol-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(FilterFirst.FilterText)}{culture}&Name={HttpUtility.UrlEncode(FilterFirst.Notes)}", forceLoad: true);
    }

    private void NavigateToDetail(ProtocolWithNavigationPropertiesDto protocol)
    {
        // Detay sayfasına yönlendirme işlemi
        NavigationManager.NavigateTo($"/protocols/detail/{protocol.Patient.PatientNumber}");
    }
    

    #region Filtreleme İşleri
    
    private void SetFiltersFirst()
    {
        FirstQuery.Queries.Params = new Dictionary<string, object>();
        FirstQuery.Queries.Params.Add("Filter", FilterFirst);
    }
    private void SetFiltersSec()
    {
        SecQuery.Queries.Params = new Dictionary<string, object>();
        SecQuery.Queries.Params.Add("Filter", FilterSec);
    }
    
    // 1.gride entegre ediyor şu an 
    private async Task HandleFilterChanged(GetProtocolsInput updatedFilter)
    {

        FilterFirst.MaxResultCount = PageSize;
        FilterFirst.SkipCount = (CurrentPage - 1) * PageSize;
        FilterFirst.Sorting = CurrentSorting;
        
        if (!IsLookupsLoaded)
        {
            await LoadLookupsAsync();
            IsLookupsLoaded = true;
        }

        SetFiltersFirst();
        await _gridRefFirst.RefreshGrid();
        await _gridRefSec.RefreshGrid();


    }
    
    // 2.grid için yazdım ->  detaylı patient listesi 
    private async Task HandleFilterChangedSec(GetProtocolsInput updatedFilter)
    {

        FilterSec.MaxResultCount = PageSize;
        FilterSec.SkipCount = (CurrentPage - 1) * PageSize;
        FilterSec.Sorting = CurrentSorting;

        SetFiltersSec();
        await _gridRefSec.RefreshGrid();
  
    }
    
    private async Task ClearFilters()
    {
        FilterFirst = new GetProtocolsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };

        StateHasChanged();
        SetFiltersFirst();
        await _gridRefFirst.RefreshGrid();
        await _gridRefSec.RefreshGrid();
    }
    
      private async Task ClearFiltersSec()
        {
            FilterSec = new GetProtocolsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting,
            };
    
            StateHasChanged();
            SetFiltersSec();
            await _gridRefSec.RefreshGrid();
        }
    
    #endregion
    
    
}
