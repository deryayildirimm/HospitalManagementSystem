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
using Pusula.Training.HealthCare.Blazor.Components.Modals;
using Pusula.Training.HealthCare.Blazor.Components.Grids;
using Volo.Abp;
using Syncfusion.Blazor.Data;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Protocol;

public partial class Protocols
{

    protected readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; set; } = new PageToolbar();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateProtocolType { get; set; }
    private bool CanEditProtocolType { get; set; }
    private bool CanDeleteProtocolType { get; set; }

    private bool _spinnerVisible;
    
    private bool IsLookupsLoaded { get; set; } 
    
    private int LookupPageSize { get; } = 50;
    
    private GenericModal<ProtocolUpdateDto> _editModal;
    private ProtocolUpdateDto _selectedProtocolType;
    private GenericGrid<ProtocolWithNavigationPropertiesDto> _gridRef;
    
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    
    private IReadOnlyList<LookupDto<Guid>> InsuranceCollections { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DoctorsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> ProtocolTypesCollection { get; set; } = [];
    private Guid EditingProtocolTypeId { get; set; }
    private GetProtocolsInput Filter { get; set; }
    private Query FilterQuery { get; set; }

    public Protocols()
    {
        _editModal = new GenericModal<ProtocolUpdateDto>();
        _gridRef = new GenericGrid<ProtocolWithNavigationPropertiesDto>();
        _selectedProtocolType = new ProtocolUpdateDto();
        Filter = new GetProtocolsInput
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
            Filter = new GetProtocolsInput();
            await SetPermissionsAsync();
            await LoadLookupsAsync();
            SetFilters();
        }
        finally
        {
            _spinnerVisible = false;
          
        }
    }

    #region Handle Filter
    
    private void SetFilters()
    {
        FilterQuery.Queries.Params = new Dictionary<string, object>();
        FilterQuery.Queries.Params.Add("Filter", Filter);
    }

    private async Task HandleFilterChanged(GetProtocolsInput updatedFilter)
    {

        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        SetFilters();
        await _gridRef.RefreshGrid();
  
    }

    private async Task ClearFilters()
    {
        Filter = new GetProtocolsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting,
        };
        StateHasChanged();
        SetFilters();
        await _gridRef.RefreshGrid();
    }
    #endregion
    
    #region Modals
    

    private async Task OpenEditModal(ProtocolWithNavigationPropertiesDto protocol)
    {

        var type = await ProtocolsAppService.GetAsync(protocol.Protocol.Id);
        
        EditingProtocolTypeId = type.Id;
        _selectedProtocolType = ObjectMapper.Map<ProtocolDto, ProtocolUpdateDto>(type);
        
        if (!IsLookupsLoaded) // Eğer daha önce yüklenmemişse verileri çek
        {
            await LoadLookupsAsync();
            IsLookupsLoaded = true; // Veriler bir kez yüklendikten sonra tekrar yüklenmesini engelle
        }

        _editModal?.Show();
    }
    
    #endregion

    #region LookUps
    
    private async Task LoadLookupsAsync()
    {
        try
        {
            DepartmentsCollection =
                (await LookupAppService.GetDepartmentLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

            InsuranceCollections =
                (await LookupAppService.GetInsuranceLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

            DoctorsCollection = (await LookupAppService.GetDoctorLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;
            ProtocolTypesCollection =
                (await LookupAppService.GetProtocolTypeLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;

        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }
    
    #endregion

    #region Initialize part
    
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Protocol"]));
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
    #endregion
    
    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProtocolsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocols/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}" , forceLoad: true);
    }

    private void NavigateToDetail(ProtocolWithNavigationPropertiesDto protocol)
    {

      
        ProtocolState.ProtocolId = protocol.Protocol.Id;
        ProtocolState.ProtocolTypeName = protocol.ProtocolType.Name;
        ProtocolState.PatientIdentityNumber = protocol.Patient.IdentityNumber;
        ProtocolState.PatientGender = protocol.Patient.Gender.ToString();
        ProtocolState.PatientBirthDate = protocol.Patient.BirthDate;
        ProtocolState.PatientId = protocol.Patient.Id;
        ProtocolState.DepartmentName = protocol.Department.Name;
        ProtocolState.StartTime = protocol.Protocol.StartTime;
        ProtocolState.DoctorName = protocol.Doctor.FirstName + " " + protocol.Doctor.LastName;
        ProtocolState.PatientName = protocol.Patient.FirstName + " " + protocol.Patient.LastName;
        ProtocolState.EndTime = protocol.Protocol.EndTime;
        
        NavigationManager.NavigateTo($"/protocols/detail/{protocol.Patient.PatientNumber}");
    }
    
    private readonly List<GridColumnDefinition> _columns = new()
    {
        new GridColumnDefinition { Field = "Patient.FirstName", HeaderText = "Patient", Width = "200px" },
        new GridColumnDefinition { Field = "Department.Name", HeaderText = "Department", Width = "200px" },
        new GridColumnDefinition { Field = "Doctor.FirstName", HeaderText = "Doctor", Width = "200px" },
        new GridColumnDefinition { Field = "ProtocolType.Name", HeaderText = "Protocol Type", Width = "200px" },
        new GridColumnDefinition { Field = "Insurance.InsuranceCompanyName", HeaderText = "Insurance", Width = "200px" },
        new GridColumnDefinition { Field = "Protocol.StartTime", HeaderText = "Start Time", Width = "200px" },
        new GridColumnDefinition { Field = "Protocol.EndTime", HeaderText = "End Time", Width = "200px" }
    
    };


    #region CRUD
  
    private async Task UpdateProtocolTypeAsync()
    {
        try
        {
            await ProtocolsAppService.UpdateAsync(EditingProtocolTypeId, _selectedProtocolType);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while deleting the Protocol Type."]);
            throw new UserFriendlyException(ex.Message);
        }
        finally
        {
            await _gridRef.RefreshGrid();
        }
          
    }
    private async Task DeleteProtocolTypeAsync(ProtocolWithNavigationPropertiesDto input)
    {
        try
        {
            await ProtocolsAppService.DeleteAsync(input.Protocol.Id);
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while deleting the Protocol ."] + ex.Message);
        }
        await _gridRef.RefreshGrid();
    }
    #endregion
}
