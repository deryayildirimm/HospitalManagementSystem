using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Pusula.Training.HealthCare.Appointments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class PatientDetail
{
   [CreatePhoneNumberValidator]
    public string MobilePhoneNumber { get; set; } = string.Empty;

    [CreatePhoneNumberValidator]
    public string RelativePhoneNumber { get; set; } = string.Empty;
    
    [Parameter]
    public int PatientNumber { get; set; }

    private PatientDto patient;
    
    private string PatientGender = "MALE";

    private bool VisibleProperty { get; set; } = true;
    
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
 
    private IReadOnlyList<PatientDto> PatientList { get; set; }
    
    private IReadOnlyList<PatientUpdateDto> EditPatient { get; set; }
    
 //   private IReadOnlyList<AppointmentWithNavigationPropertiesDto> AppointmentList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
   
    public string? RelativeCountryCode { get; set; }
    private bool AllPatientsSelected { get; set; }
    private bool CanCreatePatient { get; set; }
    private bool CanEditPatient { get; set; }
    private bool CanDeletePatient { get; set; }
    private PatientCreateDto NewPatient { get; set; }
    private PatientUpdateDto EditingPatient { get; set; }
    
    private Validations EditingPatientValidations { get; set; } = new();
    private Guid EditingPatientId { get; set; }
    private Modal EditPatientModal { get; set; } = new();
    private GetPatientsInput Filter { get; set; }
    
  //  private GetAppointmentsWithNavigationPropertiesInput FilterText { get; set; }
    
    private List<PatientDto> SelectedPatients { get; set; } = [];
    private IEnumerable<CountryPhoneCodeDto> Nationalities = [];
    private IEnumerable<KeyValuePair<int, string>> Genders = [];
    private IEnumerable<KeyValuePair<int, string>> Relatives = [];
    private IEnumerable<KeyValuePair<int, string>> PationTypes = [];
    private IEnumerable<KeyValuePair<int, string>> InsuranceTypes = [];
    private IEnumerable<KeyValuePair<int, string>> DiscountGroups = [];




    public PatientDetail()
    {
        NewPatient = new PatientCreateDto();
        EditingPatient = new PatientUpdateDto();
        Filter = new GetPatientsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        PatientList = [];
   //     AppointmentList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetPatientsAsync(); // bunu kaldırıcaz zaten test amaçlı duruyor 
        await GetPatientAsync();
        await GetAppointmentsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        //    VisibleProperty = false;
        }
    }
    /*
    public async Task OnClicked(ClickEventArgs<PatientDto> Args)
    {
        if (Args.Item.Text == "Add")
        {
       //     await Grid.AddRecordAsync();
        }
        if (Args.Item.Text == "Edit")
        {
         //   await Grid.StartEditAsync();
        }
        if (Args.Item.Text == "Delete")
        {
         //   await Grid.DeleteRecordAsync();
        }
        if (Args.Item.Text == "Update")
        {
          //  await Grid.EndEditAsync();
        }
        if (Args.Item.Text == "Cancel")
        {
           // await Grid.CloseEditAsync();
        }
    }
    */
    #region API Fetch
        private async Task GetPatientAsync()
        {
            try
            {
                // patient Number ile alıcaz normalde 
                patient = await PatientsAppService.GetPatientByNumberAsync(PatientNumber);
                PatientGender = patient.Gender.ToString();


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            GetAvatarUrl(PatientGender);
            VisibleProperty = false;

        }
        #endregion

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Patients"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);
      
        return ValueTask.CompletedTask;
    }
    
    private string GetAvatarUrl(string gender)
    {
        return gender == "FEMALE" 
            ? "/images/avatar_femalee.jpg" 
            : "/images//avatar_male.jpg";
    }

    private async Task SetPermissionsAsync()
    {
        CanCreatePatient = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Patients.Create);
        CanEditPatient = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Patients.Edit);
        CanDeletePatient = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Patients.Delete);
    }

    private async Task GetPatientsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        
        var result = await PatientsAppService.GetListAsync(Filter);
        PatientList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    #region fetching all data (appointment, doctor, medical_service)
     
    private async Task GetAppointmentsAsync()
    {
        
    //    FilterText.MaxResultCount = PageSize;
    //    FilterText.SkipCount = (CurrentPage - 1) * PageSize;
     //   FilterText.Sorting = CurrentSorting;
        
     
     // !!  FilterText.PatientId = patient.Id;
     // !! FilterText.PatientNumber = PatientNumber;
     // !! FilterText.Status = EnumAppointmentStatus.Scheduled; // yaklaşan randevular önceliğimiz 
      //  var result = await AppointmentAppService.GetListWithNavigationPropertiesAsync(FilterText);
        //doldurduk
    //    AppointmentList = result.Items;
     // diger kısımda completed, missed  bunlar da geçmiş randevular olarak listeenicek kırmızı olanlar missed olur 
     // cancelled ayrı gösterilir 
     //   TotalCount = (int)result.TotalCount; // total mıktarı ogrendık
     
     /*
      *  AppointmnetList.Doctor.Name;  -> direkt isme ulaştım bu şekilde 
      */
     
    }
    #endregion

    
    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetPatientsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await PatientsAppService.GetDownloadTokenAsync()).Token;
        var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("HealthCare") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }
        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/patients/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&FirstName={HttpUtility.UrlEncode(Filter.FirstName)}&LastName={HttpUtility.UrlEncode(Filter.LastName)}&BirthDateMin={Filter.BirthDateMin?.ToString("O")}&BirthDateMax={Filter.BirthDateMax?.ToString("O")}&IdentityNumber={HttpUtility.UrlEncode(Filter.IdentityNumber)}&EmailAddress={HttpUtility.UrlEncode(Filter.EmailAddress)}&MobilePhoneNumber={HttpUtility.UrlEncode(Filter.MobilePhoneNumber)}&Gender={Filter.Gender}", forceLoad: true);
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<PatientDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetPatientsAsync();
        await InvokeAsync(StateHasChanged);
    }
    

    private async Task OpenEditPatientModalAsync(PatientDto input)
    {
        await GetNationalitiesListAsync();
        await GetGendersListAsync();
        await GetRelativesListAsync();
        await GetPationTypesListAsync();
        await GetInsuranceTypesListAsync();
        await GetDiscountGroupsListAsync();
        await EditingPatientValidations.ClearAll();
        var editPatient = await PatientsAppService.GetAsync(input.Id);

        EditingPatientId = editPatient.Id;
        EditingPatient = ObjectMapper.Map<PatientDto, PatientUpdateDto>(editPatient);

        EditingPatient.identityField = EditingPatient.Nationality == CountryConsts.CurrentCountry ? true : false; 
        EditingPatient.passportField = EditingPatient.Nationality == CountryConsts.CurrentCountry ? false : true; 

        await EditPatientModal.Show();
    }
    


    private async Task CloseEditPatientModalAsync()
    {
        await EditPatientModal.Hide();
    }

    private async Task DeletePatientAsync(PatientDto input)
    {

        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete {input.FirstName} {input.LastName}?");
        if (!confirmed) return;

        await PatientsAppService.DeleteAsync(input.Id);
        await GetPatientsAsync();
    }

    #region randevu iptali için metod şu anlık boş

    private async Task DeleteAppointmentAsync(AppointmentDto input)
    {
        var confirmed = await UiMessageService.Confirm($"Are you sure you want to delete this ?");
        if (!confirmed) return;
        
        //
        
        
    }

    #endregion
   
    

    
    private async Task UpdatePatientAsync()
    {
        try
        {
            if (await EditingPatientValidations.ValidateAll() == false)
            {
                return;
            }

            await PatientsAppService.UpdateAsync(EditingPatientId, EditingPatient);
            await GetPatientsAsync();
            await GetPatientAsync();
            await EditPatientModal.Hide();
          
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
    
    
    private void OnRelativePhoneNumberChanged(ChangeEventArgs e)
    {
        NewPatient.RelativePhoneNumber = e.Value?.ToString();
    }

    private Task SelectAllItems()
    {
        AllPatientsSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllPatientsSelected = false;
        SelectedPatients.Clear();

        return Task.CompletedTask;
    }
    

    private Task ClearPhone()
    {

        MobilePhoneNumber = string.Empty;
        RelativePhoneNumber = string.Empty;
        RelativeCountryCode = "";
        return Task.CompletedTask;
    }

    private Task SelectedPatientRowsChanged()
    {
        if (SelectedPatients.Count != PageSize)
        {
            AllPatientsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedPatientsAsync()
    {
        var message = AllPatientsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedPatients.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllPatientsSelected)
        {
            await PatientsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await PatientsAppService.DeleteByIdsAsync(SelectedPatients.Select(x => x.Id).ToList());
        }

        SelectedPatients.Clear();
        AllPatientsSelected = false;

        await GetPatientsAsync();
    }


    private async Task GetNationalitiesListAsync()
    {
        Nationalities = await CountryAppService.GetCountryPhoneCodesAsync();
    }

    private Task GetGendersListAsync()
    {
        Genders = Enum.GetValues(typeof(EnumGender))
                      .Cast<EnumGender>()
                      .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                      .ToList();
        return Task.CompletedTask;
    }

    private Task GetRelativesListAsync()
    {
        Relatives = Enum.GetValues(typeof(EnumRelative))
                     .Cast<EnumRelative>()
                     .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                     .ToList();

        return Task.CompletedTask;
    }

    private Task GetPationTypesListAsync()
    {
        PationTypes = Enum.GetValues(typeof(EnumPatientTypes))
                             .Cast<EnumPatientTypes>()
                             .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                             .ToList();
        return Task.CompletedTask;
    }

    private Task GetInsuranceTypesListAsync()
    {
        InsuranceTypes = Enum.GetValues(typeof(EnumInsuranceType))
                     .Cast<EnumInsuranceType>()
                     .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                     .ToList();
        return Task.CompletedTask;
    }

    private Task GetDiscountGroupsListAsync()
    {
        DiscountGroups = Enum.GetValues(typeof(EnumDiscountGroup))
                     .Cast<EnumDiscountGroup>()
                     .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                     .ToList();
        return Task.CompletedTask;
    }

}
