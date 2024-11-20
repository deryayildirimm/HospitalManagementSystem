using System;
using System.Collections.Generic;
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

    private AppointmentStepperModel StepperModel { get; set; }
    private SfStepper Stepper { get; set; } = null!;
    private StepperStep SelectServiceStepper { get; set; } = null!;
    private StepperStep ScheduleStepper { get; set; } = null!;
    private StepperStep ConfirmationStepper { get; set; } = null!;
    private StepperStep ResultStepper { get; set; } = null!;
    private int ActiveStep { get; set; } = 0;

    #endregion
    
    private bool IsFinalResultSuccess { get; set; }
    private bool CanCreateAppointment { get; set; }

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
    private bool IsDoctorListLoading { get; set; }
    
    private int DoctorLoadingShimmerCount { get; set; } = 5;

    private AppointmentCreateDto NewAppointment { get; set; }

    private List<AppointmentSlotItem> AppointmentSlots { get; set; }
    
    private List<AppointmentDayItem> DaysList { get; set; }
    private int LoadCount { get; set; } = 14;
    private double ScreenWidth { get; set; } = 0;
    private int AvailableSlotCount { get; set; } = 0;
    private int LoadingShimmerCount { get; set; } = 24;
    private string AppointmentId { get; set; } = "ER123456";
    private string PaymentId { get; set; } = "PAY987654";
    private bool SlotsLoading { get; set; }
    private bool IsProcessCanceled { get; set; }

    private bool IsUserNavigatingReverse { get; set; }
    
    private bool IsCurrentStepValid  { get; set; }
    
    private bool IsFirstStepValid =>
        !string.IsNullOrEmpty(StepperModel.MedicalServiceName) &&
        !string.IsNullOrEmpty(StepperModel.DoctorName) &&
        !string.IsNullOrEmpty(StepperModel.DepartmentName);

    private bool IsSecondStepValid =>
        IsFirstStepValid &&
        !string.IsNullOrEmpty(StepperModel.AppointmentDisplayTime) &&
        !string.IsNullOrEmpty(StepperModel.AppointmentDisplayDate);

    private bool IsThirdStepValid =>
        IsSecondStepValid &&
        !string.IsNullOrEmpty(StepperModel.PatientName) &&
        !string.IsNullOrEmpty(StepperModel.HospitalName);

    public Appointments()
    {

        Patient = new PatientDto();
        GetAppointmentSlotFilter = new GetAppointmentSlotInput();
        StepperModel = new AppointmentStepperModel();
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
        IsDoctorListLoading = false;
        IsServiceListLoading = true;
        IsUserNavigatingReverse = false;
        IsCurrentStepValid = false;
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
        CanCreateAppointment = await AuthorizationService
            .IsGrantedAsync(HealthCarePermissions.Appointments.Create);
    }

    private async Task CancelProcess()
    {
        await OnStepperReset();
        ResetAppointmentInfo();
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
            var day = new AppointmentDayItem
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
            DaysList.Add(new AppointmentDayItem
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
            DaysList.Add(new AppointmentDayItem
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
        //Reset stepper model
        await OnStepperReset();
        //Reset appointment times
        ResetAppointmentInfo();

        if (args.Value.Length > 0)
        {
            var selectedService = args.Value[0];

            StepperModel.MedicalServiceId = selectedService.Id;
            StepperModel.MedicalServiceName = selectedService.DisplayName;
            StepperModel.Amount = selectedService.Cost;

            GetAppointmentSlotFilter.MedicalServiceId = selectedService.Id;
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
            StepperModel.DoctorName = item.Name;
            StepperModel.DoctorId = item.Id;
            StepperModel.DepartmentName = item.Department;
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
                
                StepperModel.PatientId = Patient.Id;
                StepperModel.PatientName = Patient.FirstName + " " + Patient.LastName;
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
        try
        {
            IsDoctorListLoading = true;
            var deptIds = MedicalServiceWithDepartmentsList
                .Where(x => x.MedicalService.Id == StepperModel.MedicalServiceId)
                .SelectMany(x => x.Departments)
                .Select(dept => dept.Id)
                .ToList();

            DoctorsWithDepartmentIdsInput.DepartmentIds = deptIds;
            var doctors = (await DoctorsAppService.GetByDepartmentIdsAsync(DoctorsWithDepartmentIdsInput)).Items;

            if (!doctors.Any())
            {
                DoctorsList = [];
                DoctorsWithDepartmentIdsInput = new GetDoctorsWithDepartmentIdsInput();
                IsDoctorListLoading = false;
                return;
            }

            DoctorsList = doctors
                .Select(x => new Doctor
                {
                    Id = x.Doctor.Id,
                    Name = $"{x.Title.TitleName} {x.Doctor.FirstName} {x.Doctor.LastName}",
                    Department = x.Department.Name,
                    Gender = x.Doctor.Gender,
                    IsAvailable = true,
                    IsSelected = false
                })
                .ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            IsDoctorListLoading = false;
        }
        
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

            AppointmentSlots = slots.Select(x => new AppointmentSlotItem
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

    private async Task OnTextChanged(string newText)
    {
        MedicalServiceFilter.Name = newText;
        await GetServices();
    }

    private async Task OnDoctorSearchChanged(string newText)
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

    private void ResetAppointmentInfo()
    {
        AvailableSlotCount = 0;
        AppointmentSlots.Clear();
        DaysList.ForEach(e => e.IsSelected = false);
        DoctorsList.ForEach(e => e.IsSelected = false);
        ServicesList.ForEach(e => e.IsSelected = false);
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
    
    private async Task OnSelectAppointmentDay(AppointmentDayItem item)
    {
        DaysList.ForEach(e => e.IsSelected = false);
        item.IsSelected = true;
        StepperModel.AppointmentDisplayDate = item.Date.ToShortDateString();
        StepperModel.AppointmentDate = item.Date;
        GetAppointmentSlotFilter.Date = item.Date;
        await GetAvailableSlots();
    }

    private void SelectAppointmentSlot(AppointmentSlotItem appointmentSlot)
    {
        if (!appointmentSlot.AvailabilityValue)
        {
            return;
        }

        if (appointmentSlot.IsSelected)
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            StepperModel.AppointmentDisplayTime = null!;
            StepperModel.StartTime = null!;
            StepperModel.EndTime = null!;
        }
        else
        {
            AppointmentSlots.ForEach(e => e.IsSelected = false);
            appointmentSlot.IsSelected = true;
            StepperModel.AppointmentDisplayTime = 
                $"{appointmentSlot.StartTime.ToString(CultureInfo.CurrentCulture)} - {appointmentSlot.EndTime.ToString(CultureInfo.CurrentCulture)}";
            StepperModel.StartTime = appointmentSlot.StartTime;
            StepperModel.EndTime = appointmentSlot.EndTime;
        }

    }

    private Task OnReminderSettingChanged(bool val)
    {
        StepperModel.ReminderSent = val;
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
            var baseDate = StepperModel.AppointmentDate;
            var parsedStartDateTime = ConvertToDateTime(baseDate, StepperModel.StartTime);
            var parsedEndDateTime = ConvertToDateTime(baseDate, StepperModel.EndTime);
        
            //New appointment object mapping
            NewAppointment.DoctorId = StepperModel.DoctorId;
            NewAppointment.PatientId = StepperModel.PatientId;
            NewAppointment.MedicalServiceId = StepperModel.MedicalServiceId;
            NewAppointment.AppointmentDate = StepperModel.AppointmentDate;
            NewAppointment.StartTime = parsedStartDateTime;
            NewAppointment.EndTime = parsedEndDateTime;
            NewAppointment.Amount = StepperModel.Amount;
            NewAppointment.Notes = StepperModel.Note;
            NewAppointment.ReminderSent = StepperModel.ReminderSent;
        
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
        
            await AppointmentAppService.CreateAsync(NewAppointment);

            IsFinalResultSuccess = true;

            await OnNextStep();
        }
        catch (Exception)
        {
            IsFinalResultSuccess = false;
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

    private async Task OnStepperReset()
    {
        ActiveStep = 0;
        await Stepper.ResetAsync();
        SelectServiceStepper.IsValid = null;
        ConfirmationStepper.IsValid = null;
        ScheduleStepper.IsValid = null;
        ResultStepper.IsValid = null;
        StepperModel = new AppointmentStepperModel
        {
            PatientId = Patient.Id,
            PatientName = Patient.FirstName + " " + Patient.LastName
        };
        StateHasChanged();
    }
    
    private void HandleStepChange(StepperChangeEventArgs args)
    {
        if (args.ActiveStep == args.PreviousStep)
        {
            return;
        }
        
        IsUserNavigatingReverse = args.ActiveStep < args.PreviousStep;
        
        if(!IsUserNavigatingReverse)
        {
            SetValidState(args);
        }
        else
        {
            for(var i = args.ActiveStep; i <= args.PreviousStep; i++)
            {
                switch (i)
                {
                    case 0:
                        SelectServiceStepper.IsValid = null;
                        break;
                    case 1:
                        ScheduleStepper.IsValid = null;
                        break;
                    case 2:
                        ConfirmationStepper.IsValid = null;
                        break;
                    case 3:
                        ResultStepper.IsValid = null;
                        break;
                }
            }
            IsCurrentStepValid = true;
        }
        
        if (IsCurrentStepValid)
        {
            ActiveStep = args.ActiveStep;
        }
        
    } 
    
    private void SetValidState (StepperChangeEventArgs args)
    {
        var activeStep = Stepper.ActiveStep;
        switch (activeStep)
        {
            case 0:
            {
                IsCurrentStepValid = IsFirstStepValid;
                SelectServiceStepper.IsValid = IsCurrentStepValid;
                break;
            }
            case 1:
            {
                IsCurrentStepValid = IsSecondStepValid;
                ScheduleStepper.IsValid = IsCurrentStepValid;
                break;
            }
            case 2:
            {
                IsCurrentStepValid = IsThirdStepValid;
                ConfirmationStepper.IsValid = IsCurrentStepValid;
                break;
            }
        }
        args.Cancel = !IsCurrentStepValid;
    }
    
    private async Task OnNextStep()
    {
        await Stepper.NextStepAsync();
    }

    private async Task OnPreviousStep()
    {
        await Stepper.PreviousStepAsync();
    }
    
    #endregion
}