using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Countries;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Blazor.Models;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages.Patient;

public partial class PatientDetail
{
    [Parameter]
    public int PatientNumber { get; set; }

    private PatientDto patient  { get; set; } = new PatientDto();

    private string PatientGender { get; set; }  = "MALE";

    private bool VisibleProperty { get; set; } = true;

    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems { get; set; } = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private List<AppointmentViewModel> AppointmentList { get; set; }
    private IReadOnlyList<AppointmentDto> FetchedAppointmentList { get; set; } = [];
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private int TotalCount { get; set; }
    private string CurrentSorting { get; set; } = string.Empty;
    private bool CanEditPatient { get; set; }
    private PatientCreateDto NewPatient { get; set; }
    private PatientUpdateDto EditingPatient { get; set; }

    private EnumAppointmentStatus? _status { get; set; } = EnumAppointmentStatus.Scheduled;

    private Validations EditingPatientValidations { get; set; } = new();
    private Guid EditingPatientId { get; set; }
    private Modal EditPatientModal { get; set; } = new();

    private GetAppointmentsInput FilterText { get; set; }

    private IEnumerable<CountryPhoneCodeDto> Nationalities { get; set; } = [];
    private IEnumerable<KeyValuePair<int, string>> Genders { get; set; }  = [];
    private IEnumerable<KeyValuePair<int, string>> Relatives { get; set; } = [];
    private IEnumerable<KeyValuePair<int, string>> PationTypes { get; set; } = [];
    private IEnumerable<KeyValuePair<int, string>> DiscountGroups { get; set; } = [];

    public PatientDetail()
    {
        NewPatient = new PatientCreateDto();
        EditingPatient = new PatientUpdateDto();
        FilterText = new GetAppointmentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };

        AppointmentList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetPatientAsync();
        await GetAppointmentsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            await InvokeAsync(StateHasChanged);

        }
    }
   
    #region Fetch Patient
    private async Task GetPatientAsync()
    {
        try
        {
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

    private string GetAvatarUrl(string gender)
    {
        return gender == "FEMALE"
            ? "/images/avatar_femalee.jpg"
            : "/images//avatar_male.jpg";
    }

    private async Task SetPermissionsAsync()
    {
        CanEditPatient = await AuthorizationService
                        .IsGrantedAsync(HealthCarePermissions.Patients.Edit);
    }

    #region DateTime formats 
    /*
     * @DateTime.Now.ToString("dd.MM.yyyy")       // 25.11.2024
     * @DateTime.Now.ToString("yyyy-MM-dd")       // 2024-11-25
     * @DateTime.Now.ToString("dd MMMM yyyy")     // 25 November 2024
     * @DateTime.Now.ToString("HH:mm:ss")         // 14:35:48
     * @DateTime.Now.ToString("dd.MM.yyyy HH:mm") // 25.11.2024 14:35
     */


    #endregion



    #region fetching all data (appointment, doctor, medical_service)

    private async Task GetAppointmentsAsync()
    {

        FilterText.PatientNumber = PatientNumber;

        FilterText.Status = _status;
        var apps = (await AppointmentAppService.GetListAsync(FilterText)).Items;

        AppointmentList = apps.Select(x => new AppointmentViewModel
        {

            PatientName = x.Patient?.FirstName + " " + x.Patient?.LastName ?? "Unknown",
            DoctorName = x.Doctor?.FirstName + " " + x.Doctor?.LastName ?? "Unknown",
            Date = x?.AppointmentDate.Date.ToString("dd MMMM yyyy") ?? DateTime.MinValue.ToString("dd MMMM yyyy"),
            Status = x?.Status ?? EnumAppointmentStatus.Scheduled,
            Service = x?.MedicalService?.Name ?? "Not Available"

        }).ToList();

        TotalCount = (int)apps.Count;

        FetchedAppointmentList = apps;


    }
    #endregion
    
    private async Task OpenEditPatientModalAsync(PatientDto input)
    {
        await GetNationalitiesListAsync();
        await GetGendersListAsync();
        await GetRelativesListAsync();
        await GetPationTypesListAsync();
        await GetDiscountGroupsListAsync();
        await EditingPatientValidations.ClearAll();
        var editPatient = await PatientsAppService.GetAsync(input.Id);

        EditingPatientId = editPatient.Id;
        EditingPatient = ObjectMapper.Map<PatientDto, PatientUpdateDto>(editPatient);

        await EditPatientModal.Show();
    }

    private async Task CloseEditPatientModalAsync()
    {
        await EditPatientModal.Hide();
    }



    #region statuye göre veri çekme 

    protected virtual async Task CompletedApp()
    {
        _status = EnumAppointmentStatus.Completed;
        await GetAppointmentsAsync();
    }

    protected virtual async Task CancelledApp()
    {
        _status = EnumAppointmentStatus.Cancelled;
        await GetAppointmentsAsync();
    }
    protected virtual async Task ScheduledApp()
    {
        _status = EnumAppointmentStatus.Scheduled;
        await GetAppointmentsAsync();
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

    private Task GetDiscountGroupsListAsync()
    {
        DiscountGroups = Enum.GetValues(typeof(EnumDiscountGroup))
                     .Cast<EnumDiscountGroup>()
                     .Select(e => new KeyValuePair<int, string>((int)e, e.ToString()))
                     .ToList();
        return Task.CompletedTask;
    }

    #region Fake Data For MedicalCondition

    private List<MedicalConditionViewModel> MedicalConditions = new()
    {
        new MedicalConditionViewModel
        {
            DiseaseName = "Diabetes",
            DiagnosisDate = new DateTime(2020, 5, 15),
            TreatmentStatus = "Ongoing",
            DoctorNotes = "Patient needs to monitor blood sugar levels regularly."
        },
        new MedicalConditionViewModel
        {
            DiseaseName = "Hypertension",
            DiagnosisDate = new DateTime(2019, 11, 20),
            TreatmentStatus = "Under Control",
            DoctorNotes = "Low-sodium diet and regular exercise recommended."
        },
        new MedicalConditionViewModel
        {
            DiseaseName = "Asthma",
            DiagnosisDate = new DateTime(2021, 3, 10),
            TreatmentStatus = "Stable",
            DoctorNotes = "Inhaler prescribed for emergencies."
        }
    };

    public class MedicalConditionViewModel
    {
        public string? DiseaseName { get; set; }
        public DateTime? DiagnosisDate { get; set; }
        public string? TreatmentStatus { get; set; }
        public string? DoctorNotes { get; set; }
    }

    #endregion

}
