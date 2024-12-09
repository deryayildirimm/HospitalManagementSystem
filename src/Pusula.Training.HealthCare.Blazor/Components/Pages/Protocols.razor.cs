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
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Domain.Entities;


namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Protocols
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; } 
    private IReadOnlyList<ProtocolWithNavigationPropertiesDto> ProtocolList { get; set; }
    
    private IReadOnlyList<PatientDto> PatientList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private bool CanCreateProtocol { get; set; }
    private bool CanEditProtocol { get; set; }
    private bool CanDeleteProtocol { get; set; }
    
    private PatientDto EditingPatient { get; set; } 
    private ProtocolCreateDto NewProtocol { get; set; }
    private Validations NewProtocolValidations { get; set; } = new();
    private ProtocolUpdateDto EditingProtocol { get; set; }
    private Validations EditingProtocolValidations { get; set; } = new();
    private Guid EditingProtocolId { get; set; }
    private Modal CreateProtocolModal { get; set; } = new();
    private Modal EditProtocolModal { get; set; } = new();
    private GetProtocolsInput Filter { get; set; }
    
     private GetPatientsInput FilterPatient { get; set; }
    private DataGridEntityActionsColumn<ProtocolWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "protocol-create-tab";
    protected string SelectedEditTab = "protocol-edit-tab";
    
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    
    private IReadOnlyList<LookupDto<Guid>> InsuranceCollections { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> DoctorsCollection { get; set; } = [];
    private IReadOnlyList<LookupDto<Guid>> ProtocolTypesCollection { get; set; } = [];
    private List<ProtocolWithNavigationPropertiesDto> SelectedProtocols { get; set; } = [];

    private bool AllProtocolsSelected { get; set; }

    public Protocols()
    {
        NewProtocol = new ProtocolCreateDto();
        EditingProtocol = new ProtocolUpdateDto();
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
        await SetPermissionsAsync();

    }
    
    private bool IsLookupsLoaded { get; set; } = false; 
    
    protected  async Task LoadLookupsAsync()
    {
        
            await GetDepartmentCollectionLookupAsync();
            await GetDoctorCollectionLookupAsync();
            await GetProtocolTypeCollectionLookupAsync();
            await GetInsuranceCollectionLookupAsync();
        
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Protocols"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        Toolbar.AddButton(L["NewProtocol"], OpenCreateProtocolModalAsync, IconName.Add, requiredPolicyName: HealthCarePermissions.Protocols.Create);

        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateProtocol = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Protocols.Create);
        CanEditProtocol = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Protocols.Edit);
        CanDeleteProtocol = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Protocols.Delete);


    }
    
    

    private async Task GetProtocolsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await ProtocolsAppService.GetListAsync(Filter);
        ProtocolList = result.Items;
        EditingPatient = ProtocolList[0].Patient;
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

    private async Task OpenCreateProtocolModalAsync()
    {
        NewProtocol = new ProtocolCreateDto
        {
            StartTime = DateTime.Now,
        };

        SelectedCreateTab = "protocol-create-tab";
        
        if (!IsLookupsLoaded) 
        {
            await LoadLookupsAsync();
            IsLookupsLoaded = true; // Veriler bir kez yüklendikten sonra tekrar yüklenmesini engelle
        }


        await NewProtocolValidations.ClearAll();
        await CreateProtocolModal.Show();
    }

    private async Task CloseCreateProtocolModalAsync()
    {
        NewProtocol = new ProtocolCreateDto
        {
            StartTime = DateTime.Now,

            DepartmentId = DepartmentsCollection.Select(i => i.Id).FirstOrDefault(),

        };
        
        //resetle
        IdentityNumber = string.Empty;
        FoundPatientName = string.Empty;
        NewPatient = new PatientCreateDto();

        await CreateProtocolModal.Hide();
    }

    private async Task OpenEditProtocolModalAsync(ProtocolWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "protocol-edit-tab";
        
        if (!IsLookupsLoaded) // Eğer daha önce yüklenmemişse verileri çek
        {
            await LoadLookupsAsync();
            IsLookupsLoaded = true; // Veriler bir kez yüklendikten sonra tekrar yüklenmesini engelle
        }
        
        var protocol = await ProtocolsAppService.GetWithNavigationPropertiesAsync(input.Protocol.Id);

        EditingProtocolId = protocol.Id;
        EditingPatient = input.Patient;
        EditingProtocol = ObjectMapper.Map<ProtocolDto, ProtocolUpdateDto>(protocol);

        await EditingProtocolValidations.ClearAll();
        await EditProtocolModal.Show();
    }

    private async Task DeleteProtocolAsync(ProtocolWithNavigationPropertiesDto input)
    {
        await ProtocolsAppService.DeleteAsync(input.Protocol.Id);
        await GetProtocolsAsync();
    }

    protected void NavigateToDetail(ProtocolWithNavigationPropertiesDto protocolDto)
    {
        //seçili hastayı servise kaydediyoruz 
        //   patientService.SetPatient(patientDto);
        NavigationManager.NavigateTo($"/protocols/detail/{protocolDto.Protocol.Patient.PatientNumber}");

    }
    
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
                IdentityAndPassportNumber = IdentityNumber // take the identitiy number 
            };
            // warn the user 
            await UiMessageService.Warn("No patient found. Please fill in the patient details.");
        }
    }

    private async Task CreateProtocolAsync()
    {

        if (await NewProtocolValidations.ValidateAll() == false) return;
            try
            {
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
                NewProtocol.PatientId = patient.Id;
                await ProtocolsAppService.CreateAsync(NewProtocol);

                await UiMessageService.Success("Protocol created successfully!");

                await GetProtocolsAsync();
                await CloseCreateProtocolModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        
    }

    private async Task CloseEditProtocolModalAsync()
    {
        await EditProtocolModal.Hide();
    }

    private async Task UpdateProtocolAsync()
    {
        try
        {
            if (await EditingProtocolValidations.ValidateAll() == false)
            {
                return;
            }

            await ProtocolsAppService.UpdateAsync(EditingProtocolId, EditingProtocol);
            await GetProtocolsAsync();
            await EditProtocolModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
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
    protected virtual async Task OnDepartmentIdChangedAsync(Guid? departmentId)
    {
        Filter.DepartmentId = departmentId;
        
        await SearchAsync();
    }
    
    protected virtual async Task OnInsuranceIdChangedAsync(Guid? insuranceId)
    {
        Filter.InsuranceId = insuranceId;
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

    private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
    {
        DepartmentsCollection = (await ProtocolsAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task GetInsuranceCollectionLookupAsync(string? newValue = null)
    {
        InsuranceCollections = (await ProtocolsAppService.GetInsuranceLookUpAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task GetDoctorCollectionLookupAsync(string? newValue = null)
    {
        DoctorsCollection = (await ProtocolsAppService.GetDoctorLookUpAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    
    private async Task GetProtocolTypeCollectionLookupAsync(string? newValue = null)
    {
        ProtocolTypesCollection = (await ProtocolsAppService.GetProtocolTypeLookUpAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    

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


}
