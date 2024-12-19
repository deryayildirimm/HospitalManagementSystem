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

public partial class CreateAppointment : HealthCareComponentBase
{
    private DateTime CurrentDate { get; set; }
    private int LookupPageSize { get; } = 100;
    private bool IsDoctorsEnabled { get; set; }
    private GetDoctorsWithDepartmentIdsInput DoctorsWithDepartmentIdsInput { get; set; }
    private IReadOnlyList<LookupDto<Guid>> AppointmentTypesCollection { get; set; }
    private IReadOnlyList<MedicalServiceDto> MedicalServiceCollection { get; set; }
    private List<DoctorLookupDto> DoctorsCollection { get; set; }
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
    private int DoctorPageSize { get; } = 50;
    private int DoctorCurrentPage { get; set; } = 1;
    private string DoctorCurrentSorting { get; set; } = string.Empty;
    private AppointmentCreateDto NewAppointment { get; set; }
    private AppointmentUpdateDto EditingAppointment { get; set; }
    private int AppointmentPageSize { get; set; } = 50;
    private int AppointmentCurrentPage { get; set; } = 1;
    private string DoctorNameInfo { get; set; } = string.Empty;
    private string MedicalServiceNameInfo { get; set; } = string.Empty;
    private bool IsVisibleSearchPatient { get; set; }

    private PatientCreateDto NewPatient { get; set; }
    private List<KeyValuePair<string, EnumGender>> GendersCollection { get; set; }
    private SfSchedule<AppointmentCustomData> ScheduleObj { get; set; }

    private GetAppointmentsLookupInput DaysLookupFilter { get; set; }
    private GetAppointmentByDateInput AppointmentByDateInput { get; set; }
    private int LoadCount { get; set; } = 14;
    private List<AppointmentDayItemLookupDto> DaysLookupList { get; set; }

    private bool IsCreateAppointmentOpen { get; set; }
    private bool IsEditAppointmentOpen { get; set; }
    private SfToast ToastObj { get; set; }
    private string ToastContent { get; set; } = "";
    private string ToastTitle { get; set; } = "Information";
    private string ToastCssClass { get; set; } = "";
    private int AvailableSlotCount { get; set; } = 0;
    private Guid EditingAppointmentId { get; set; } = default;

    private bool IsSlotSearchAvailable =>
        IsIdValid(NewAppointment.MedicalServiceId) &&
        IsIdValid(NewAppointment.DoctorId);

    private bool IsCreateAppointmentModelValid =>
        IsIdValid(NewAppointment.MedicalServiceId) &&
        IsIdValid(NewAppointment.DepartmentId) &&
        IsIdValid(NewAppointment.DoctorId) &&
        IsIdValid(NewAppointment.PatientId);
    
    private bool IsCreateAppointmentValid =>
        IsCreateAppointmentModelValid &&
        IsIdValid(NewAppointment.AppointmentTypeId);

    private bool IsSelectedPatientValid
        =>
            IsIdValid(SelectedPatient?.Id) &&
            !string.IsNullOrWhiteSpace(SelectedPatient?.FirstName)
            && !string.IsNullOrWhiteSpace(SelectedPatient?.LastName);

    private static bool IsIdValid(Guid? id) =>
        id.HasValue && id.Value != Guid.Empty;

    public CreateAppointment()
    {
        SelectedPatient = new PatientDto();
        PatientGrid = new SfGrid<PatientDto>();
        ToastObj = new SfToast();
        CurrentDate = DateTime.Now;
        AppointmentTypesCollection = [];
        MedicalServiceCollection = [];
        MedicalServiceWithDepartmentsList = [];
        GendersCollection = [];
        SlotItems = [];
        DaysLookupList = [];
        PatientCollection = [];
        DoctorsCollection = [];
        IsDoctorsEnabled = false;
        IsVisibleSearchPatient = false;
        IsCreateAppointmentOpen = false;
        IsEditAppointmentOpen = false;
        ScheduleObj = new SfSchedule<AppointmentCustomData>();
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

        DaysLookupFilter = new GetAppointmentsLookupInput
        {
            Offset = LoadCount,
            StartDate = DateTime.Now
        };

        AppointmentByDateInput = new GetAppointmentByDateInput();
        GetAppointmentSlotFilter = new GetAppointmentSlotInput();
        NewAppointment = new AppointmentCreateDto();
        NewPatient = new PatientCreateDto();
        EditingAppointment = new AppointmentUpdateDto();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetLookupsAsync();
        await GetMedicalServices();
        SetGenders();
    }

    #region Lookups

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
            await UiMessageService.Error(e.Message);
        }
    }

    private void SetGenders()
    {
        GendersCollection = Enum.GetValues(typeof(EnumGender))
            .Cast<EnumGender>()
            .Select(e => new KeyValuePair<string, EnumGender>(e.ToString(), e))
            .ToList();
    }

    #endregion

    #region ApiCalls

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
                .ToList();
        }
        catch (Exception e)
        {
            MedicalServiceCollection = [];
            MedicalServiceWithDepartmentsList = [];
            await UiMessageService.Error(e.Message);
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
                ObjectMapper.Map<List<DoctorWithNavigationPropertiesDto>, List<DoctorLookupDto>>(doctors);
        }
        catch (Exception e)
        {
            IsDoctorsEnabled = false;
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetAppointmentSlots()
    {
        try
        {
            GetAppointmentSlotFilter.MedicalServiceId = NewAppointment.MedicalServiceId;
            GetAppointmentSlotFilter.DoctorId = NewAppointment.DoctorId;
            CurrentDate = GetAppointmentSlotFilter.Date;

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
                DateOnly = x.Date.Date,
                StartTime = x.Date.Date.Add(TimeSpan.Parse(x.StartTime)),
                EndTime = x.Date.Date.Add(TimeSpan.Parse(x.EndTime)),
                IsReadOnly = x.AvailabilityValue,
            }).ToList();

            AvailableSlotCount = slots.Count(x => x.AvailabilityValue);
        }
        catch (BusinessException e)
        {
            SlotItems = [];
            AvailableSlotCount = 0;
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

    private async Task RegisterPatient()
    {
        try
        {
            await PatientsAppService.CreateAsync(NewPatient);
            ToastContent = @L["PatientCreated"];
            ToastTitle = L["Information"];
            ToastCssClass = "e-toast-info";
            await ShowOnClick();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task GetAppointmentDays()
    {
        try
        {
            if (!IsSlotSearchAvailable)
            {
                return;
            }

            DaysLookupList.Clear();

            DaysLookupFilter.DoctorId = NewAppointment.DoctorId;
            DaysLookupFilter.MedicalServiceId = NewAppointment.MedicalServiceId;

            var days =
                (await AppointmentAppService.GetAvailableDaysLookupAsync(DaysLookupFilter))
                .Items
                .ToList();

            DaysLookupList = ObjectMapper.Map<List<AppointmentDayLookupDto>, List<AppointmentDayItemLookupDto>>(days);
            StateHasChanged();
        }
        catch (Exception e)
        {
            DaysLookupList = [];
            await UiMessageService.Error(e.Message);
        }
    }

    private async Task UpdateAppointmentAsync()
    {
        try
        {
            await AppointmentAppService.UpdateAsync(EditingAppointmentId, EditingAppointment);
            CloseEditAppointmentModal();
        }
        catch (Exception ex)
        {
            await UiMessageService.Error(ex.Message);
        }
        finally
        {
            CloseEditAppointmentModal();
        }
    }

    private async Task OnValidSubmitNewAppointment()
    {
        try
        {
            if (!IsCreateAppointmentValid)
            {
                ToastContent = @L[$"FillRequiredFields"];
                ToastTitle = @L["ValidationError"];
                ToastCssClass = "e-toast-danger";
                StateHasChanged();
                await ShowOnClick();
                return;
            }
            
            await AppointmentAppService.CreateAsync(NewAppointment);

            ToastContent = $"{@L[$"OperationSuccessful"]}\n{@L["AppointmentInformationWillBeSent"]}";
            ToastTitle = @L["AppointmentCreated"];
            ToastCssClass = "e-toast-success";
            IsCreateAppointmentOpen = false;
            StateHasChanged();
            await ShowOnClick();
            ResetAppointmentInfo();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
        finally
        {
            await GetAppointmentSlots();
            await GetAppointmentDays();
            ScheduleObj.CloseEditor();
            IsCreateAppointmentOpen = false;
        }
    }

    #endregion

    #region DialogModalControllers

    private async Task OpenEditDialog(AppointmentCustomData input, DateTime appointmentDate, DateTime newStartTime,
        DateTime newEndDate)
    {
        try
        {
            AppointmentByDateInput.DoctorId = GetAppointmentSlotFilter.DoctorId;
            AppointmentByDateInput.MedicalServiceId = GetAppointmentSlotFilter.MedicalServiceId;
            AppointmentByDateInput.AppointmentDate = GetAppointmentSlotFilter.Date;

            var item = await AppointmentAppService.GetByDateAsync(AppointmentByDateInput);
            EditingAppointmentId = item.Id;
            EditingAppointment = ObjectMapper.Map<AppointmentDto, AppointmentUpdateDto>(item);
            EditingAppointment.StartTime = newStartTime;
            EditingAppointment.EndTime = newEndDate;
            EditingAppointment.AppointmentDate = appointmentDate;
            IsEditAppointmentOpen = true;
        }
        catch (Exception e)
        {
            CloseEditAppointmentModal();
            await UiMessageService.Error(e.Message);
        }
    }

    private void CloseEditAppointmentModal()
    {
        EditingAppointment = new AppointmentUpdateDto();
        AppointmentByDateInput = new GetAppointmentByDateInput();
        EditingAppointmentId = Guid.Empty;
        IsEditAppointmentOpen = false;
    }

    #endregion

    #region SchedulerHandlers

    private async void OnPopupOpen(PopupOpenEventArgs<AppointmentCustomData> args)
    {
        if (!args.Data.IsReadOnly)
        {
            IsCreateAppointmentOpen = false;
            args.Cancel = true;
            return;
        }

        if (args.Type is PopupType.Editor or PopupType.QuickInfo && !IsCreateAppointmentModelValid)
        {
            args.Cancel = true;
            ToastTitle = L["Error"];
            ToastContent = @L["PatientDoctorServiceError"];
            ToastCssClass = "e-toast-danger";
            StateHasChanged();
            await ShowOnClick();
            return;
        }

        if (args.Type is PopupType.Editor or PopupType.QuickInfo)
        {
            IsCreateAppointmentOpen = false;
            args.Duration = 60;
        }

        if (args.Type is PopupType.Editor)
        {
            args.Cancel = true;
            NewAppointment.StartTime = args.Data.StartTime;
            NewAppointment.EndTime = args.Data.EndTime;
            NewAppointment.AppointmentDate = args.Data.DateOnly;
            IsCreateAppointmentOpen = true;
        }
    }

    private async Task OnNavigateDate(NavigatingEventArgs args)
    {
        if (args.ActionType is ActionType.DateNavigate && !IsSlotSearchAvailable)
        {
            args.Cancel = true;
            return;
        }

        if (args.ActionType is ActionType.DateNavigate && IsSlotSearchAvailable)
        {
            GetAppointmentSlotFilter.Date = args.CurrentDate;

            await GetAppointmentSlots();
        }
    }

    public void OnDragStart(Syncfusion.Blazor.Schedule.DragEventArgs<AppointmentCustomData> args)
    {
        args.Scroll.Enable = false;
        if (args.Data.IsReadOnly)
        {
            AppointmentByDateInput.StartTime = DateTime.Now;
            AppointmentByDateInput.EndTime = DateTime.Now;
            args.Cancel = true;
        }

        AppointmentByDateInput.StartTime = args.StartTime;
        AppointmentByDateInput.EndTime = args.EndTime;
    }

    public async Task OnDragged(Syncfusion.Blazor.Schedule.DragEventArgs<AppointmentCustomData> args)
    {
        args.Cancel = false;
        if (args.Data == null || args.Data.IsReadOnly)
        {
            return;
        }

        await OpenEditDialog(args.Data, args.Data.DateOnly, args.StartTime, args.EndTime);
    }

    #endregion

    private List<Guid> GetRelevantDepartmentIds(Guid? medicalServiceId) =>
        MedicalServiceWithDepartmentsList
            .Where(x => x.MedicalService.Id == medicalServiceId)
            .SelectMany(x => x.Departments)
            .Select(dept => dept.Id)
            .ToList();

    private async Task OnMedicalServiceChange(SelectEventArgs<MedicalServiceDto> args)
    {
        try
        {
            NewAppointment.MedicalServiceId = args.ItemData.Id;
            MedicalServiceNameInfo = args.ItemData.Name;
            NewAppointment.Amount = args.ItemData.Cost;
            await GetDoctorsList();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private async void OnDoctorChange(SelectEventArgs<DoctorLookupDto> args)
    {
        try
        {
            NewAppointment.DoctorId = args.ItemData.Id;
            NewAppointment.DepartmentId = args.ItemData.DepartmentId;
            DoctorNameInfo = args.ItemData.DisplayName;
            await GetAppointmentDays();
        }
        catch (Exception e)
        {
            await UiMessageService.Error(e.Message);
        }
    }

    private void ResetAppointmentInfo()
    {
        NewAppointment.StartTime = DateTime.Today;
        NewAppointment.EndTime = DateTime.Today;
        NewAppointment.AppointmentTypeId = Guid.Empty;
        NewAppointment.ReminderSent = true;
        NewAppointment.Notes = null;
    }

    private async void OnMoreDetailsClick(MouseEventArgs args, AppointmentCustomData data)
    {
        await ScheduleObj.CloseQuickInfoPopupAsync();
        await ScheduleObj.OpenEditorAsync(data, CurrentAction.Add);
    }

    private static string GetEventDetails(AppointmentCustomData? data)
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

        NewAppointment.PatientId = patient.Id;
        ClosePatientSearchModal();
    }

    private async Task ShowOnClick()
    {
        await ToastObj.ShowAsync();
    }

    private void ClearFilters()
    {
        SelectedPatient = new PatientDto();
        NewAppointment = new AppointmentCreateDto();
        MedicalServiceNameInfo = "";
        StateHasChanged();
    }

    private async Task OnLoadDaysDaysLeft()
    {
        var newStartDate = DaysLookupFilter.StartDate.AddDays(-LoadCount);
        DaysLookupFilter.StartDate = newStartDate;
        await GetAppointmentDays();
    }

    private async Task OnLoadDaysRight()
    {
        DaysLookupFilter.StartDate = DaysLookupFilter.StartDate.AddDays(LoadCount);
        await GetAppointmentDays();
    }

    private async Task OnSelectAppointmentDay(AppointmentDayItemLookupDto item)
    {
        DaysLookupList.ForEach(e => e.IsSelected = false);
        item.IsSelected = true;

        CurrentDate = item.Date;
        GetAppointmentSlotFilter.Date = item.Date;
        await GetAppointmentSlots();
    }
}