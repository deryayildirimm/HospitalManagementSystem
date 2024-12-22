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
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Syncfusion.Blazor.Data;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Domain.Entities;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Protocol;

public partial class Protocols
{

    protected readonly List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanCreateProtocolType { get; set; }
    private bool CanEditProtocolType { get; set; }
    private bool CanDeleteProtocolType { get; set; }

    private bool _spinnerVisible;
    
    private bool IsLookupsLoaded { get; set; } 
    
    private int LookupPageSize { get; } = 50;

    private GenericModal<ProtocolCreateDto> _createModal;
    private GenericModal<ProtocolUpdateDto> _editModal;
    private ProtocolCreateDto _newProtocol;
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
        _createModal = new GenericModal<ProtocolCreateDto>();
        _editModal = new GenericModal<ProtocolUpdateDto>();
        _gridRef = new GenericGrid<ProtocolWithNavigationPropertiesDto>();
        _newProtocol = new ProtocolCreateDto();
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
    
    private  Task ShowModal()
    {
        _newProtocol = new ProtocolCreateDto();
        _createModal?.Show();
        return  Task.CompletedTask;
    }

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
        Toolbar.AddButton(L["Create Protocol"], ShowModal, IconName.Add, requiredPolicyName: HealthCarePermissions.Protocols.Create);
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
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/protocol-types/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Notes)}", forceLoad: true);
    }

    private void NavigateToDetail(ProtocolWithNavigationPropertiesDto protocol)
    {
       
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
    
    private string IdentityNumber { get; set; } = string.Empty; // Kimlik Numarası Alanı
    private string FoundPatientName { get; set; } = string.Empty; // Bulunan hastanın adı (eğer varsa)
    private PatientCreateDto NewPatient { get; set; } = new(); // Yeni hasta bilgileri
    private bool IsPatientFound { get; set; } = true; // Hasta bulunup bulunmadığını takip eder
    
    
    private async Task OnCheckIdentityNumberClicked()
    {
        
        if (string.IsNullOrWhiteSpace(IdentityNumber) )
        {
            await UiMessageService.Warn("Please enter an Identity Number.");
            return;
        }
        try
        {
            // check the patient
            var patient = await PatientsAppService.GetPatientByIdentityAsync(IdentityNumber);
            FoundPatientName = $"{patient.FirstName} {patient.LastName}";
            IsPatientFound = true;
           
        }
        catch (EntityNotFoundException)
        {
            // if patient cannot found 
            IsPatientFound = false;
         
            FoundPatientName = string.Empty;
            NewPatient = new PatientCreateDto
            {
                IdentityNumber = IdentityNumber // take the identitiy number 
            };
            // warn the user 
            await UiMessageService.Warn("No patient found. Please fill in the patient details.");
        }
    }
    private async Task CreateProtocolTypeAsync(ProtocolCreateDto protocol)
    {
        
        try
        {
            if (!IsLookupsLoaded) // Eğer daha önce yüklenmemişse verileri çek
            {
                await LoadLookupsAsync();
                IsLookupsLoaded = true; // Veriler bir kez yüklendikten sonra tekrar yüklenmesini engelle
            }
          
            PatientDto patient;
            
            if (!IsPatientFound)
            {
                // creating new patient 
                patient = await PatientsAppService.CreateAsync(NewPatient);
            }
            else
            {
                // Zaten bulunan hasta bilgisi alınır
                patient = await PatientsAppService.GetPatientByIdentityAsync(IdentityNumber);
                
            }
            
            // create the protocol
            _newProtocol.PatientId = patient.Id;
            protocol = _newProtocol;
            await ProtocolsAppService.CreateAsync(protocol);

            await UiMessageService.Success("Protocol created successfully!");

            await _gridRef.RefreshGrid();
           
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(@L["An error occurred while creating the Protocol."] + ex.Message);
            throw new UserFriendlyException(ex.Message);
        }

    
       
    }

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
