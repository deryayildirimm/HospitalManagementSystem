using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Departments;
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

    private StepperModel AppointmentStepperModel;
    private SfStepper Stepper;
    private StepperStep SelectServiceStepper;
    private StepperStep ScheduleStepper;
    private StepperStep ConfirmationStepper;
    private StepperStep ResultStepper;
    private int ActiveStep = 0;

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

    private GetAppointmentsInput GetAppointmentSlotFilter { get; set; }

    #endregion

    private List<SelectionItem> ServicesList { get; set; }

    private SfListBox<SelectionItem[], SelectionItem> SelectServiceDropdown;

    private IReadOnlyList<MedicalServiceWithDepartmentsDto> MedicalServiceWithDepartmentsList { get; set; }

    private List<Doctor> DoctorsList { get; set; }

    private AppointmentCreateDto NewAppointment { get; set; }

    private List<AppointmentSlot> AppointmentSlots { get; set; }
    
    private Random random = new Random();
    private List<DayItem> DaysList { get; set; }
    private int loadCount = 14;
    private int currentDayOffset = 0;
    private double screenWidth = 0;
    private int AvailableSlotCount { get; set; } = 0;
    private int LoadingShimmerCount { get; set; } = 24;

    private string appointmentNote = "";
    private string selectedChannel = "";
    private bool isActive = true;
    private string appointmentId = "ER123456";
    private string paymentId = "PAY987654";
    private bool IsDoctorWorking = true;
    private string DoctorWorkingInfo = "";
    private string AppointmentSlotsErrorMessage = "";
    private bool SlotsLoading { get; set; }
    
    private bool IsProcessCanceled { get; set; }

    private bool isValid = false;
    private string content = "";
    private bool isValidMsg = false;
    private bool showFeedBack = false;


    private bool IsFirstStepValid =>
        !string.IsNullOrEmpty(AppointmentStepperModel.MedicalServiceName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.DoctorName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.DepartmentName);

    private bool IsSecondStepValid =>
        IsFirstStepValid &&
        !string.IsNullOrEmpty(AppointmentStepperModel.AppointmentTime) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.AppointmentDate);

    private bool IsThirdStepValid =>
        IsSecondStepValid &&
        !string.IsNullOrEmpty(AppointmentStepperModel.PatientName) &&
        !string.IsNullOrEmpty(AppointmentStepperModel.HospitalName);

    public Appointments()
    {

        MedicalServiceFilter = new GetMedicalServiceInput
        {
            Name = "",
            MaxResultCount = ServicePageSize,
            SkipCount = (ServiceCurrentPage - 1) * ServicePageSize,
            Sorting = ServiceCurrentSorting
        };

        NewAppointment = new AppointmentCreateDto
        {
        };

        DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput
        {
            Name = "",
            MaxResultCount = DoctorPageSize,
            SkipCount = (DoctorCurrentPage - 1) * DoctorPageSize,
            Sorting = DoctorCurrentSorting
        };

        GetAppointmentSlotFilter = new GetAppointmentsInput();

        AppointmentStepperModel = new StepperModel();

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
        await GetServices();
        AddInitialDays();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            screenWidth = await JS.InvokeAsync<double>("getWindowSize");
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
        loadCount = screenWidth switch
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
        if (lastDay.AddDays(-loadCount) < DateTime.Now)
        {
            AddInitialDays();
            return;
        }
        
        DaysList.Clear();

        for (var i = 0; i < loadCount; i++)
        {
            lastDay = lastDay.Date.AddDays(-1);
            var day = new DayItem
            {
                Date = lastDay,
                IsSelected = false,
                IsAvailable = random.Next(0, 2) == 0
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
        
        for (var i = 0; i < loadCount; i++)
        {
            DaysList.Add(new DayItem
            {
                Date = lastDay.Date.AddDays(i),
                IsSelected = false,
                IsAvailable = random.Next(0, 2) == 0
            });
        }

        StateHasChanged();
    }

    private void AddInitialDays()
    {
        DaysList.Clear();
        for (var i = 0; i < loadCount; i++)
        {
            DaysList.Add(new DayItem
            {
                Date = DateTime.Now.AddDays(i),
                IsSelected = false,
                IsAvailable = random.Next(0, 2) == 0
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
            NewAppointment = new AppointmentCreateDto();
            AppointmentStepperModel = new StepperModel();
        }
        else
        {
            var item = args.Value[0];
            NewAppointment.MedicalServiceId = item.Id;
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
            .Where(x => x.MedicalService.Id == NewAppointment.MedicalServiceId)
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
        AppointmentStepperModel.AppointmentDate = item.Date.ToShortDateString();
        GetAppointmentSlotFilter.Date = item.Date;
        await GetAvailableSlots();
    }

    private void SelectAppointment(AppointmentSlot appointmentSlot)
    {
        if (!appointmentSlot.AvailabilityValue)
        {
            return;
        }

        if (appointmentSlot.IsSelected)
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            AppointmentStepperModel.AppointmentTime = null;
        }
        else
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            appointmentSlot.IsSelected = true;
            AppointmentStepperModel.AppointmentTime = 
                $"{appointmentSlot.StartTime.ToString(CultureInfo.CurrentCulture)} - {appointmentSlot.EndTime.ToString(CultureInfo.CurrentCulture)}";
        }

        AppointmentStepperModel.PatientId = Guid.NewGuid();

    }

    //TODO Create will be done
    private async Task CreateAppointment()
    {
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
        
        OnNextStep();

        IsFinalResultSuccess = true;
    }

    #region StepHandlers

    private async void OnStepperReset()
    {
        ActiveStep = 0;
        await Stepper.ResetAsync();
        AppointmentStepperModel = new StepperModel();
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

        [Required] public string PatientName { get; set; } = "John Doe";

        [Required] public Guid PatientId { get; set; }

        [Required] public string AppointmentDate { get; set; }

        [Required] public string AppointmentTime { get; set; }

        [Required] public string DoctorName { get; set; }

        [Required] public Guid DoctorId { get; set; }

        [Required] public string DepartmentName { get; set; }

        [Required] public string MedicalServiceName { get; set; }
        
        public string? Note { get; set; }

        [Required] public bool ReminderSent { get; set; }

        [Required] public Guid MedicalServiceId { get; set; }

        [Required] public double Amount { get; set; } = 0.0;
    }

    #endregion
}