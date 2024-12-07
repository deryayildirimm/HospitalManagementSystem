using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Shared;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Schedule;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class CreateAppointment
{
    private DateTime CurrentDate { get; set; }
    private int LookupPageSize { get; } = 100;
    private bool IsDoctorsEnabled { get; set; }
    private GetDoctorsWithDepartmentIdsInput DoctorsWithDepartmentIdsInput { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<LookupDto<Guid>> MedicalServiceCollection { get; set; }
    private List<LookupDto<Guid>> DoctorsCollection { get; set; }
    private IReadOnlyList<AppointmentDto> DoctorsAppointments { get; set; }
    private IReadOnlyList<AppointmentCustomData> SlotItems { get; set; }
    private IReadOnlyList<PatientDto> PatientCollection { get; set; }
    private SfGrid<PatientDto> PatientGrid { get; set; }

    private PatientDto SelectedPatient { get; set; }
    private IReadOnlyList<MedicalServiceWithDepartmentsDto> MedicalServiceWithDepartmentsList { get; set; }
    private GetAppointmentsInput AppointmentsFilter { get; set; }
    private GetMedicalServiceInput MedicalServiceFilter { get; set; }
    private GetAppointmentSlotInput GetAppointmentSlotFilter { get; set; }
    private GetPatientsInput GetPatientsInput { get; set; }
    private int PatientPageSize { get; set; } = 10;
    private int PatientCurrentPage { get; set; } = 1;
    private int ServicePageSize { get; } = 50;
    private int ServiceCurrentPage { get; set; } = 1;
    private string ServiceCurrentSorting { get; set; } = string.Empty;
    private int TypePageSize { get; } = 50;
    private int DoctorPageSize { get; } = 50;
    private int DoctorCurrentPage { get; set; } = 1;
    private string DoctorCurrentSorting { get; set; } = string.Empty;
    private AppointmentCreateDto NewAppointment { get; set; }
    private int AppointmentPageSize { get; set; } = 50;
    private int AppointmentCurrentPage { get; set; } = 1;
    private string DoctorNameInfo { get; set; } = string.Empty;

    private bool IsVisibleSearchPatient { get; set; }

    private PatientCreateDto NewPatient { get; set; }
    private List<KeyValuePair<string, EnumGender>> GendersCollection { get; set; }
    private SfSchedule<AppointmentCustomData> ScheduleObj { get; set; }

    private SfToast ToastObj { get; set; }
    private string ToastPosition { get; set; } = "";
    private string ToastContent { get; set; } = "";

    private bool IsSlotSearchAvailable =>
        IsAppointmentTypeIdValid(NewAppointment.MedicalServiceId) &&
        IsAppointmentTypeIdValid(NewAppointment.DoctorId);

    private static bool IsAppointmentTypeIdValid(Guid? appointmentTypeId) =>
        appointmentTypeId.HasValue && appointmentTypeId.Value != Guid.Empty;

    public CreateAppointment()
    {
        CurrentDate = DateTime.Now;
        AppointmentTypesCollection = [];
        MedicalServiceCollection = [];
        MedicalServiceWithDepartmentsList = [];
        DoctorsAppointments = [];
        GendersCollection = [];
        SlotItems = [];
        PatientCollection = [];
        IsDoctorsEnabled = false;
        IsVisibleSearchPatient = false;
        ScheduleObj = new SfSchedule<AppointmentCustomData>();
        ToastObj = new SfToast();
        DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput
        {
            Name = "",
            MaxResultCount = DoctorPageSize,
            SkipCount = (DoctorCurrentPage - 1) * DoctorPageSize,
            Sorting = DoctorCurrentSorting
        };

        MedicalServiceFilter = new GetMedicalServiceInput
        {
            Name = "",
            MaxResultCount = ServicePageSize,
            SkipCount = (ServiceCurrentPage - 1) * ServicePageSize,
            Sorting = ServiceCurrentSorting
        };

        AppointmentsFilter = new GetAppointmentsInput
        {
            AppointmentMinDate = DateTime.Today,
            AppointmentMaxDate = DateTime.Today.AddDays(7),
            MaxResultCount = AppointmentPageSize,
            SkipCount = (AppointmentCurrentPage - 1) * AppointmentPageSize,
        };

        GetPatientsInput = new GetPatientsInput
        {
            Sorting = "PatientNumber ASC",
            MaxResultCount = PatientPageSize,
            SkipCount = (PatientCurrentPage - 1) * PatientPageSize,
        };

        GetAppointmentSlotFilter = new GetAppointmentSlotInput();
        NewAppointment = new AppointmentCreateDto();
        NewPatient = new PatientCreateDto();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetLookupsAsync();
        await GetMedicalServices();
        SetGenders();
    }

    private async Task SetLookupsAsync()
    {
        try
        {
            AppointmentTypesCollection =
                (await LookupAppService.GetAppointmentTypeLookupAsync(new LookupRequestDto
                    { MaxResultCount = LookupPageSize }))
                .Items;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    private void SetGenders()
    {
        GendersCollection = Enum.GetValues(typeof(EnumGender))
            .Cast<EnumGender>()
            .Select(e => new KeyValuePair<string, EnumGender>(e.ToString(), e))
            .ToList();
    }

    private async Task GetMedicalServices()
    {
        try
        {
            MedicalServiceWithDepartmentsList =
                (await MedicalServicesAppService
                    .GetMedicalServiceWithDepartmentsAsync(MedicalServiceFilter))
                .Items
                .ToList();

            MedicalServiceCollection = MedicalServiceWithDepartmentsList
                .Select(x => x.MedicalService)
                .Select(x => new LookupDto<Guid>
                {
                    Id = x.Id,
                    DisplayName = x.Name
                }).ToList();

            NewAppointment.MedicalServiceId = MedicalServiceCollection[0].Id;
            await GetDoctorsList();
        }
        catch (Exception e)
        {
            MedicalServiceCollection = [];
            MedicalServiceWithDepartmentsList = [];
            throw new UserFriendlyException(e.Message);
        }
    }

    private async Task GetDoctorsList()
    {
        try
        {
            var deptIds = GetRelevantDepartmentIds(NewAppointment.MedicalServiceId);

            DoctorsWithDepartmentIdsInput.DepartmentIds = deptIds;
            var doctors =
                (await DoctorsAppService.GetByDepartmentIdsAsync(DoctorsWithDepartmentIdsInput))
                .Items
                .ToList();

            if (doctors.Count == 0)
            {
                DoctorsCollection = [];
                DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput();
                IsDoctorsEnabled = false;
                return;
            }

            IsDoctorsEnabled = true;

            DoctorsCollection =
                ObjectMapper.Map<List<DoctorWithNavigationPropertiesDto>, List<LookupDto<Guid>>>(doctors);
            NewAppointment.DoctorId = DoctorsCollection[0].Id;
        }
        catch (Exception e)
        {
            IsDoctorsEnabled = false;
            await UiMessageService.Error(e.Message);
        }
    }

    private List<Guid> GetRelevantDepartmentIds(Guid? medicalServiceId) =>
        MedicalServiceWithDepartmentsList
            .Where(x => x.MedicalService.Id == medicalServiceId)
            .SelectMany(x => x.Departments)
            .Select(dept => dept.Id)
            .ToList();

    private async Task OnMedicalServiceChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        IsDoctorsEnabled = true;

        try
        {
            NewAppointment.MedicalServiceId = args.ItemData.Id;
            await GetDoctorsList();
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    private async Task OnDoctorChange(SelectEventArgs<LookupDto<Guid>> args)
    {
        IsDoctorsEnabled = true;

        try
        {
            NewAppointment.DoctorId = args.ItemData.Id;
            DoctorNameInfo = args.ItemData.DisplayName;
            await GetAppointments();
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    private async Task GetAppointments()
    {
        try
        {
            var items = (await AppointmentAppService.GetListAsync(AppointmentsFilter))
                .Items
                .ToList();

            if (items.Count == 0)
            {
                DoctorsAppointments = [];
            }

            DoctorsAppointments = items;
        }
        catch (Exception e)
        {
            DoctorsAppointments = [];
            throw new UserFriendlyException(e.Message);
        }
    }

    private async Task GetAppointmentSlots()
    {
        try
        {
            GetAppointmentSlotFilter.MedicalServiceId = NewAppointment.MedicalServiceId;
            GetAppointmentSlotFilter.DoctorId = NewAppointment.DoctorId;

            var slots = (await AppointmentAppService.GetAvailableSlotsAsync(GetAppointmentSlotFilter))
                .Items
                .ToList();

            if (slots.Count == 0)
            {
                SlotItems = [];
                return;
            }

            SlotItems = slots.Select(x => new AppointmentCustomData
            {
                DateOnly = x.Date,
                StartTime = x.Date.Date.Add(TimeSpan.Parse(x.StartTime)),
                EndTime = x.Date.Date.Add(TimeSpan.Parse(x.EndTime)),
                IsReadOnly = x.AvailabilityValue,
            }).ToList();
        }
        catch (BusinessException e)
        {
            SlotItems = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetPatients()
    {
        try
        {
            var patients =
                (await PatientsAppService.GetListAsync(GetPatientsInput))
                .Items
                .ToList();

            if (patients.Count == 0)
            {
                PatientCollection = [];
                return;
            }

            PatientCollection = patients;
        }
        catch (BusinessException e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private void OnPopupOpen(PopupOpenEventArgs<AppointmentCustomData> args)
    {
        if (!args.Data.IsReadOnly)
        {
            args.Cancel = true;
        }

        if (args.Type is PopupType.Editor or PopupType.QuickInfo)
        {
            args.Duration = 60;
        }
    }

    private async void OnMoreDetailsClick(MouseEventArgs args, AppointmentCustomData data)
    {
        await ScheduleObj.CloseQuickInfoPopupAsync();

        await ScheduleObj.OpenEditorAsync(data, CurrentAction.Add);
    }

    private string GetEventDetails(AppointmentCustomData? data)
    {
        return data?.StartTime + " - " + data?.EndTime;
    }

    private void ClosePatientSearchModal()
    {
        IsVisibleSearchPatient = false;
    }

    private void OpenPatientSearchModal()
    {
        IsVisibleSearchPatient = true;
    }

    private void SelectPatient(PatientDto? patient)
    {
        if (patient == null)
        {
            return;
        }

        SelectedPatient = patient;

        ClosePatientSearchModal();
    }

    private async Task RegisterPatient()
    {
        try
        {
            await PatientsAppService.CreateAsync(NewPatient);
            ToastContent = @L["PatientCreated"];
            await ShowOnClick();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task ShowOnClick()
    {
        await ToastObj.ShowAsync();
    }

    private async Task HideOnClick()
    {
        await ToastObj.HideAsync("All");
    }
}