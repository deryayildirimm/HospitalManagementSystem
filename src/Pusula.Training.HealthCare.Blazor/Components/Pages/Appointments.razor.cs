using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Navigations;
using Volo.Abp;
using Doctor = Pusula.Training.HealthCare.Blazor.Models.Doctor;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Appointments
{
    [Parameter] public int PatientNo { get; set; }

    #region Stepper

    private StepperModel AppointmentStepperModel { get; set; }
    private SfStepper Stepper { get; set; } = null!;
    private StepperStep SelectServiceStepper { get; set; } = null!;
    private StepperStep ScheduleStepper { get; set; } = null!;
    private StepperStep ConfirmationStepper { get; set; } = null!;
    private StepperStep ResultStepper { get; set; } = null!;
    private int ActiveStep { get; set; } = 0;

    #endregion
    
    private bool IsFinalResultSuccess { get; set; }
    private bool CanCreateDepartment { get; set; }
    private bool CanEditDepartment { get; set; }
    private bool CanDeleteDepartment { get; set; }

    #region MedicalServiceFilters

    private GetMedicalServiceInput MedicalServiceFilter { get; set; }
    private int ServicePageSize { get; } = 50;
    private int ServiceCurrentPage { get; set; } = 1;
    private string ServiceCurrentSorting { get; set; } = string.Empty;
    private bool IsServiceListLoading { get; set; }

    #endregion

    #region DoctorFilters

    private GetDoctorsWithDepartmentIdsInput DoctorsWithDepartmentIdsInput { get; set; }
    private int DoctorPageSize { get; } = 50;
    private int DoctorCurrentPage { get; set; } = 1;
    private string DoctorCurrentSorting { get; set; } = string.Empty;

    #endregion

    #region AppointmentFilters

    private GetAppointmentSlotInput GetAppointmentSlotFilter { get; set; }

    #endregion

    private PatientDto Patient { get; set; }
    private List<SelectionItem> ServicesList { get; set; }

    private SfListBox<SelectionItem[], SelectionItem> SelectServiceDropdown { get; set; } = null!;

    private IReadOnlyList<MedicalServiceWithDepartmentsDto> MedicalServiceWithDepartmentsList { get; set; } = null!;

    private List<Doctor> DoctorsList { get; set; }

    private AppointmentCreateDto NewAppointment { get; set; }

    private List<AppointmentSlot> AppointmentSlots { get; set; }
    
    private List<DayItem> DaysList { get; set; }
    private int LoadCount { get; set; } = 14;
    private double ScreenWidth { get; set; } = 0;
    private int AvailableSlotCount { get; set; } = 0;
    private int LoadingShimmerCount { get; set; } = 24;
    private string AppointmentId { get; set; } = "ER123456";
    private string PaymentId { get; set; } = "PAY987654";
    private bool SlotsLoading { get; set; }
    private bool IsProcessCanceled { get; set; }

    private bool IsFirstStepValid =>
        !string.IsNullOrEmpty(AppointmentStepperModel.MedicalServiceName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.DoctorName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.DepartmentName);

    private bool IsSecondStepValid =>
        IsFirstStepValid &&
        !string.IsNullOrEmpty(AppointmentStepperModel.AppointmentDisplayTime) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.AppointmentDisplayDate);

    private bool IsThirdStepValid =>
        IsSecondStepValid &&
        !string.IsNullOrEmpty(AppointmentStepperModel.PatientName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.HospitalName);

    public Appointments()
    {

        Patient = new PatientDto();
        GetAppointmentSlotFilter = new GetAppointmentSlotInput();
        AppointmentStepperModel = new StepperModel();
        NewAppointment = new AppointmentCreateDto();
        MedicalServiceFilter = new GetMedicalServiceInput
        {
            Name = "",
            MaxResultCount = ServicePageSize,
            SkipCount = (ServiceCurrentPage - 1) * ServicePageSize,
            Sorting = ServiceCurrentSorting
        };
        DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput
        {
            Name = "",
            MaxResultCount = DoctorPageSize,
            SkipCount = (DoctorCurrentPage - 1) * DoctorPageSize,
            Sorting = DoctorCurrentSorting
        };
        DoctorsList = [];
        ServicesList = [];
        DaysList = [];
        AppointmentSlots = [];
        IsFinalResultSuccess = false;
        IsProcessCanceled = false;
        IsServiceListLoading = true;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetPatient();
        await GetServices();
        AddInitialDays();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            try
            {
                ScreenWidth = await JS.InvokeAsync<double>("getWindowSize");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetDayLoadCount();
            AddInitialDays();
        }
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateDepartment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Create);
        CanEditDepartment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Edit);
        CanDeleteDepartment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Delete);
    }

    private async Task CancelProcess()
    {
        IsProcessCanceled = true;
        OnStepperReset();
        await ResetAppointment();
        IsProcessCanceled = false;
        StateHasChanged();
    }

    #region GenerateAppointmentDays
    private void SetDayLoadCount()
    {
        LoadCount = ScreenWidth switch
        {
            < 768 => 7,
            < 1024 => 14,
            _ => 14
        };
    }
    private void LoadMoreDaysLeft()
    {
        if (DaysList.Count == 0)
        {
            AddInitialDays();
            return;
        }
        
        var lastDay = DaysList[0].Date;

        //if new days smaller than today, then reset the days array
        if (lastDay.AddDays(-LoadCount) < DateTime.Now)
        {
            AddInitialDays();
            return;
        }
        
        DaysList.Clear();

        for (var i = 0; i < LoadCount; i++)
        {
            lastDay = lastDay.Date.AddDays(-1);
            var day = new DayItem
            {
                Date = lastDay,
                IsSelected = false,
                IsAvailable = true
            };

            DaysList.Insert(0, day);
        }

        StateHasChanged();
    }

    private void LoadMoreDaysRight()
    {
        if (DaysList.Count == 0)
        {
            AddInitialDays();
            return;
        }

        var lastDay = DaysList.Last();
        DaysList.Clear();
        
        for (var i = 0; i < LoadCount; i++)
        {
            DaysList.Add(new DayItem
            {
                Date = lastDay.Date.AddDays(i),
                IsSelected = false,
                IsAvailable = true
            });
        }

        StateHasChanged();
    }

    private void AddInitialDays()
    {
        DaysList.Clear();
        for (var i = 0; i < LoadCount; i++)
        {
            DaysList.Add(new DayItem
            {
                Date = DateTime.Now.AddDays(i),
                IsSelected = false,
                IsAvailable = true
            });
        }

        DaysList[0].IsSelected = true;
        StateHasChanged();
    }
    
    #endregion
    
    #region Selections

    private async Task OnServiceChange(ListBoxChangeEventArgs<SelectionItem[], SelectionItem> args)
    {
        if (args.Value.Length == 0)
        {
            OnStepperReset();
        }
        else
        {
            var item = args.Value[0];
            AppointmentStepperModel.MedicalServiceId = item.Id;
            AppointmentStepperModel.MedicalServiceName = item.DisplayName;
            AppointmentStepperModel.Amount = item.Cost;
            GetAppointmentSlotFilter.MedicalServiceId = item.Id;
            
        }

        await GetDoctorsList();
    }

    private void OnDoctorSelect(Doctor item)
    {
        if (item.IsSelected)
        {
            DoctorsList.ForEach(e => e.IsSelected = false);
        }
        else
        {
            DoctorsList.ForEach(e => e.IsSelected = false);
            item.IsSelected = true;
        }

        if (item.IsSelected)
        {
            AppointmentStepperModel.DoctorName = item.Name;
            AppointmentStepperModel.DoctorId = item.Id;
            AppointmentStepperModel.DepartmentName = item.Department;
            GetAppointmentSlotFilter.DoctorId = item.Id;
        }

        StateHasChanged();
    }

    #endregion

    #region API Fetch

    private async Task GetPatient()
    {
        try
        {
            var patientFilter = new GetPatientsInput
            {
                PatientNumber = PatientNo
            };
            
            var patients =  (await PatientsAppService.GetListAsync(patientFilter)).Items;

            if (patients.Count > 0)
            {
                Patient = patients[0];
                
                AppointmentStepperModel.PatientId = Patient.Id;
                AppointmentStepperModel.PatientName = Patient.FirstName + " " + Patient.LastName;
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task GetServices()
    {
        try
        {
            IsServiceListLoading = true;

            var services =
                (await MedicalServiceAppService
                    .GetMedicalServiceWithDepartmentsAsync(MedicalServiceFilter)).Items;

            MedicalServiceWithDepartmentsList = services.ToList();

            await ClearServiceSelection();
            ServicesList = [];
            ServicesList = services.Select(x => x.MedicalService)
                .Select(y => new SelectionItem
                {
                    DisplayName = y.Name,
                    Id = y.Id,
                    Cost = y.Cost,
                    IsSelected = false
                }).ToList();

            StateHasChanged();
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
        finally
        {
            IsServiceListLoading = false;
        }
    }

    private async Task GetDoctorsList()
    {
        var deptIds = MedicalServiceWithDepartmentsList
            .Where(x => x.MedicalService.Id == AppointmentStepperModel.MedicalServiceId)
            .SelectMany(x => x.Departments)
            .Select(dept => dept.Id)
            .ToList();

        DoctorsWithDepartmentIdsInput.DepartmentIds = deptIds;
        var doctors = (await DoctorsAppService.GetByDepartmentIdsAsync(DoctorsWithDepartmentIdsInput)).Items;

        if (!doctors.Any())
        {
            DoctorsList = [];
            DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput();
            return;
        }

        DoctorsList = doctors
            .Select(x => new Doctor
            {
                Id = x.Doctor.Id,
                Name = $"{x.Title.TitleName} {x.Doctor.FirstName} {x.Doctor.LastName}",
                Department = x.Department.Name,
                Gender = x.Doctor.Gender == EnumGender.MALE ? @L["Gender:M"] : @L["Gender:F"],
                IsAvailable = true,
                IsSelected = false
            })
            .ToList();
    }

    private async Task GetAvailableSlots()
    {
        try
        {
            SlotsLoading = true;

            var slots =
                (await AppointmentAppService.GetAvailableSlotsAsync(GetAppointmentSlotFilter)).Items;

            if (!slots.Any())
            {
                AppointmentSlots = [];
                return;
            }

            AppointmentSlots = slots.Select(x => new AppointmentSlot
            {
                DoctorId = x.DoctorId,
                MedicalServiceId = x.MedicalServiceId,
                Date = x.Date,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                AvailabilityValue = x.AvailabilityValue,
                IsSelected = false
            }).ToList();

            AvailableSlotCount = AppointmentSlots.Count(e => e.AvailabilityValue);
        }
        catch (BusinessException e)
        {
            AppointmentSlots = [];
            AvailableSlotCount = 0;
        }
        finally
        {
            SlotsLoading = false;
        }
        
    }

    #endregion

    #region SearchData

    private async void OnTextChanged(string? newText)
    {
        MedicalServiceFilter.Name = newText;
        await GetServices();
    }

    private async void OnDoctorSearchChanged(string? newText)
    {
        DoctorsWithDepartmentIdsInput.Name = newText;
        await GetDoctorsList();
    }

    #endregion

    #region ClearingHandlers

    private async void ClearServiceSearch()
    {
        MedicalServiceFilter.Name = "";
        await GetServices();
    }

    private async Task ClearServiceSelection()
    {
        await SelectServiceDropdown.SelectItemsAsync(SelectServiceDropdown.Value, false);
    }

    private Task ResetAppointment()
    {
        AvailableSlotCount = 0;

        AppointmentSlots.Clear();
        DaysList.ForEach(e => e.IsSelected = false);
        DoctorsList.ForEach(e => e.IsSelected = false);
        ServicesList.ForEach(e => e.IsSelected = false);
        return Task.CompletedTask;
    }

    #endregion

    #region StyleHandler
    private string GetDoctorCardClass(Doctor doctor)
    {
        var classes = new List<string>
        {
            "rounded-1",
            "shadow",
            "p-2",
            "m-0",
            "bg-white",
            "rounded",
            "doctor-card"
        };

        if (doctor.IsSelected)
        {
            classes.Add("doctor-card-active");
        }

        if (!doctor.IsAvailable)
        {
            classes.Add("disabled-card");
        }

        return string.Join(" ", classes);
    }
    

    #endregion
    
    private async Task OnSelectAppointmentDay(DayItem item)
    {
        DaysList.ForEach(e => e.IsSelected = false);
        item.IsSelected = true;
        AppointmentStepperModel.AppointmentDisplayDate = item.Date.ToShortDateString();
        AppointmentStepperModel.AppointmentDate = item.Date;
        GetAppointmentSlotFilter.Date = item.Date;
        await GetAvailableSlots();
    }

    private void SelectAppointmentSlot(AppointmentSlot appointmentSlot)
    {
        if (!appointmentSlot.AvailabilityValue)
        {
            return;
        }

        if (appointmentSlot.IsSelected)
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            AppointmentStepperModel.AppointmentDisplayTime = null!;
            AppointmentStepperModel.StartTime = null!;
            AppointmentStepperModel.EndTime = null!;
        }
        else
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            appointmentSlot.IsSelected = true;
            AppointmentStepperModel.AppointmentDisplayTime = 
                $"{appointmentSlot.StartTime.ToString(CultureInfo.CurrentCulture)} - {appointmentSlot.EndTime.ToString(CultureInfo.CurrentCulture)}";
            AppointmentStepperModel.StartTime = appointmentSlot.StartTime;
            AppointmentStepperModel.EndTime = appointmentSlot.EndTime;
        }

    }

    private Task OnReminderSettingChanged(bool val)
    {
        AppointmentStepperModel.ReminderSent = val;
        return Task.CompletedTask;
    }
    
    private async Task CreateAppointment()
    {

        if (!IsThirdStepValid)
        {
            return;
        }

        try
        {
            var baseDate = AppointmentStepperModel.AppointmentDate;
            var parsedStartDateTime = ConvertToDateTime(AppointmentStepperModel.AppointmentDate, AppointmentStepperModel.StartTime);
            var parsedEndDateTime = ConvertToDateTime(AppointmentStepperModel.AppointmentDate, AppointmentStepperModel.EndTime);
        
            //New appointment object mapping
            NewAppointment.DoctorId = AppointmentStepperModel.DoctorId;
            NewAppointment.PatientId = AppointmentStepperModel.PatientId;
            NewAppointment.MedicalServiceId = AppointmentStepperModel.MedicalServiceId;
            NewAppointment.AppointmentDate = AppointmentStepperModel.AppointmentDate;
            NewAppointment.StartTime = parsedStartDateTime;
            NewAppointment.EndTime = parsedEndDateTime;
            NewAppointment.Amount = AppointmentStepperModel.Amount;
            NewAppointment.Notes = AppointmentStepperModel.Note;
            NewAppointment.ReminderSent = AppointmentStepperModel.ReminderSent;
        
            var message = L["ConfirmMessage"];
            var confirm = L["Confirm"];
        
            if (!await UiMessageService.Confirm(message, confirm, options =>
                {
                    options.ConfirmButtonText = L["Yes"];
                    options.CancelButtonText = L["No"];
                }))
            {
                return;
            }
        
            var it = await AppointmentAppService.CreateAsync(NewAppointment);

            IsFinalResultSuccess = it != null;
            
            OnNextStep();

            IsFinalResultSuccess = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private DateTime ConvertToDateTime(DateTime appointmentDate, string timeString)
    {
        if (TimeSpan.TryParse(timeString, out var time))
        {
            return appointmentDate.Date.Add(time);
        }
        
        throw new FormatException("Invalid time format. Expected format is 'hh:mm'.");

    }

    #region StepHandlers

    private async void OnStepperReset()
    {
        ActiveStep = 0;
        await Stepper.ResetAsync();
        AppointmentStepperModel = new StepperModel
        {
            PatientId = Patient.Id,
            PatientName = Patient.FirstName + " " + Patient.LastName
        };
        StateHasChanged();
    }

    private void HandleStepChange(StepperChangedEventArgs args)
    {
        if (args.ActiveStep == args.PreviousStep)
        {
            return;
        }

        //Go to previous step
        if (args.ActiveStep < args.PreviousStep)
        {
            OnStepperReset();
            ActiveStep = args.ActiveStep;
            return;
        }

        if (ValidateStep(args.PreviousStep))
        {
            SetStepValid(args.PreviousStep, true);
            ActiveStep = args.ActiveStep;
        }
        else
        {
            SetStepValid(args.PreviousStep, false);
        }
    }

    private bool ValidateStep(int stepIndex)
    {
        switch (stepIndex)
        {
            case 0:
                return IsFirstStepValid;
            case 1:
                return IsSecondStepValid;
            case 2:
                return IsThirdStepValid;
            default:
                return false;
        }
    }

    private void SetStepValid(int stepIndex, bool isValid)
    {
        switch (stepIndex)
        {
            case 0:
                SelectServiceStepper.IsValid = isValid;
                break;
            case 1:
                ScheduleStepper.IsValid = isValid;
                break;
            case 2:
                ConfirmationStepper.IsValid = isValid;
                break;
            case 3:
                ResultStepper.IsValid = isValid;
                break;
        }
    }

    private async void OnNextStep()
    {
        await Stepper.NextStepAsync();
    }

    private async void OnPreviousStep()
    {
        await Stepper.PreviousStepAsync();
    }

    private bool ValidateCurrentStep()
    {
        bool isStepValid = true;
        var context = new ValidationContext(AppointmentStepperModel);
        var validationResults = new List<ValidationResult>();
        isStepValid = Validator.TryValidateObject(AppointmentStepperModel, context, validationResults, true);

        if (ActiveStep == 1)
        {
        }

        return isStepValid;
    }

    #endregion

    #region Models

    public class DayItem
    {
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AppointmentSlot
    {
        public Guid DoctorId { get; set; }
        public Guid MedicalServiceId { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool AvailabilityValue { get; set; }
        public bool IsSelected { get; set; } = false;

    }

    public class StepperModel
    {
        [Required] public string HospitalName { get; set; } = "XYZ Hospital";

        [Required] 
        public string PatientName { get; set; } = null!;

        [Required] 
        public Guid PatientId { get; set; }

        [Required] 
        public string AppointmentDisplayDate { get; set; } = null!;
        
        [Required] 
        public DateTime AppointmentDate { get; set; }

        [Required] 
        public string AppointmentDisplayTime { get; set; }
        
        [Required]
        public string StartTime { get; set; } = null!;
    
        [Required]
        public string EndTime { get; set; } = null!;

        [Required] 
        public string DoctorName { get; set; } = null!;

        [Required] 
        public Guid DoctorId { get; set; }

        [Required] 
        public string DepartmentName { get; set; } = null!;

        [Required] 
        public string MedicalServiceName { get; set; } = null!;
        
        public string? Note { get; set; }

        [Required] public bool ReminderSent { get; set; }

        [Required] public Guid MedicalServiceId { get; set; }

        [Required] public double Amount { get; set; } = 0.0;
    }

    #endregion
}