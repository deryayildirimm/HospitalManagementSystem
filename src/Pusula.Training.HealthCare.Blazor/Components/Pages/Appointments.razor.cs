using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Permissions;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class Appointments
{
    [Parameter] public int PatientNo { get; set; }

    private Steps stepsRef;
    private DateTime? selectedDay;
    private bool IsFinalResultSuccess { get; set; }
    private bool CanCreateDepartment { get; set; }
    private bool CanEditDepartment { get; set; }
    private bool CanDeleteDepartment { get; set; }
    private string selectedStep { get; set; } = "select_service";

    private List<SelectionItem> ServicesList = new List<SelectionItem>
    {
        new SelectionItem { DisplayName = "General Consultation", IsSelected = false, Cost = 1112 },
        new SelectionItem { DisplayName = "Blood Test", IsSelected = false, Cost = 1212 },
        new SelectionItem { DisplayName = "X-Ray Imaging", IsSelected = false, Cost = 112 },
        new SelectionItem { DisplayName = "MRI Scan", IsSelected = false, Cost = 1812 },
        new SelectionItem { DisplayName = "Ultrasound", IsSelected = false, Cost = 1512 },
        new SelectionItem { DisplayName = "Physical Therapy", IsSelected = false, Cost = 122 },
        new SelectionItem { DisplayName = "Vaccination", IsSelected = false, Cost = 1112 },
        new SelectionItem { DisplayName = "Cardiology Check-Up", IsSelected = false, Cost = 144 },
        new SelectionItem { DisplayName = "Dental Cleaning", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "MRI Scan", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "Ultrasound", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "Physical Therapy", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "Vaccination", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "Cardiology Check-Up", IsSelected = false, Cost = 1177 },
        new SelectionItem { DisplayName = "Dental Cleaning", IsSelected = false, Cost = 1177 },
    };

    private List<Doctor> DoctorsList = new()
    {
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a01b"), Name = "Dr. Ahmet Yıldız", Department = "Neurology",
            Gender = "Male",
            IsAvailable = true, InsuranceType = "Private", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a02b"), Name = "Dr. Zeynep Demirtaş",
            Department = "Pediatrics", Gender = "Female",
            IsAvailable = false, InsuranceType = "SGK", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a03b"), Name = "Dr. Hasan Korkmaz",
            Department = "Orthopedics", Gender = "Male",
            IsAvailable = true, InsuranceType = "Private", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a04b"), Name = "Dr. Melis Kaya", Department = "Dermatology",
            Gender = "Female",
            IsAvailable = true, InsuranceType = "Private", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a05b"), Name = "Dr. Cengiz Erdoğan",
            Department = "Psychiatry", Gender = "Male",
            IsAvailable = false, InsuranceType = "SGK", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a06b"), Name = "Dr. Burcu Tekin",
            Department = "Endocrinology", Gender = "Female",
            IsAvailable = true, InsuranceType = "Private", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a07b"), Name = "Dr. Baran Kılıç", Department = "Urology",
            Gender = "Male",
            IsAvailable = true, InsuranceType = "SGK", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a08b"), Name = "Dr. Sibel Aydın", Department = "Cardiology",
            Gender = "Female",
            IsAvailable = true, InsuranceType = "Private", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a09b"), Name = "Dr. Kemal Yalçın",
            Department = "General Surgery", Gender = "Male",
            IsAvailable = false, InsuranceType = "Senior", IsSelected = false
        },
        new Doctor
        {
            Id = new Guid("d28888e9-2ba9-4d9b-8e75-8f9c98b1a0ab"), Name = "Dr. Emine Aksoy",
            Department = "Internal Medicine", Gender = "Female",
            IsAvailable = true, InsuranceType = "SGK", IsSelected = false
        }
    };

    private AppointmentSummaryItem AppointmentSummary { get; set; }

    private List<AppointmentTime> AppointmentTimes { get; set; }

    private SelectedService selectedService { get; set; }
    private Random random = new Random();
    private List<DayItem> DaysList { get; set; }
    private int loadCount = 14;
    private int currentDayOffset = 0;
    private double screenWidth = 0;
    private int AvailableSlotCount { get; set; } = 0;

    private string appointmentNote = "";
    private string selectedChannel = "";
    private bool isActive = true;
    private string appointmentId = "ER123456";
    private string paymentId = "PAY987654";
    private bool IsProcessCanceled { get; set; }

    private bool IsFullFilledService =>
        !string.IsNullOrEmpty(selectedService.ServiceName) &&
        !string.IsNullOrEmpty(selectedService.DoctorName) &&
        !string.IsNullOrEmpty(selectedService.DepartmentName);

    private bool IsAppointmentTimeValid =>
        IsFullFilledService &&
        !string.IsNullOrEmpty(AppointmentSummary.AppointmentDate) &&
        !string.IsNullOrEmpty(AppointmentSummary.AppointmentTime);

    private bool IsAppointmentValid =>
        IsAppointmentTimeValid &&
        !string.IsNullOrEmpty(AppointmentSummary.PatientName) &&
        !string.IsNullOrEmpty(AppointmentSummary.HospitalName) &&
        !string.IsNullOrEmpty(AppointmentSummary.AppointmentDate) &&
        !string.IsNullOrEmpty(AppointmentSummary.AppointmentTime) &&
        !string.IsNullOrEmpty(AppointmentSummary.DoctorName) &&
        !string.IsNullOrEmpty(AppointmentSummary.Department) &&
        !string.IsNullOrEmpty(AppointmentSummary.ServiceType);

    public Appointments()
    {
        selectedService = new SelectedService
        {
            DepartmentName = null,
            DoctorName = null,
            ServiceName = null,
        };
        DaysList = new List<DayItem>();
        AppointmentTimes = new List<AppointmentTime>();
        AppointmentSummary = new AppointmentSummaryItem();
        IsFinalResultSuccess = false;
        IsProcessCanceled = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        AddInitialDays();
        GenerateAppointmentTimes();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            screenWidth = await JS.InvokeAsync<double>("getWindowSize");
            SetLoadCount();
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
        await ResetAppointment();
        await stepsRef.SelectStep("select_service");
        IsProcessCanceled = false;
        StateHasChanged();
    }

    private Task ResetAppointment()
    {
        
        selectedService.ServiceName = null;
        selectedService.DoctorName = null;
        selectedService.DepartmentName = null;
        selectedService.Cost = 0.0;
        
        AppointmentSummary.AppointmentTime = null;
        AppointmentSummary.AppointmentDate = null;
        AppointmentSummary.ServiceType = null;
        AppointmentSummary.Price = 0.0;
        AppointmentSummary.DoctorName = null;
        AppointmentSummary.Department = null;
        AvailableSlotCount = 0;
        
        AppointmentTimes.Clear();
        DaysList.ForEach(e => e.IsSelected = false);
        DoctorsList.ForEach(e => e.IsSelected = false);
        ServicesList.ForEach(e => e.IsSelected = false);
        return Task.CompletedTask;
    }

    private void LoadMoreDaysLeft()
    {
        if (DaysList.Count == 0)
        {
            AddInitialDays();
            return;
        }

        var lastDay = DaysList[0].Date;

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
        AppointmentSummary.AppointmentDate = DaysList[0].Date.ToShortDateString();
        StateHasChanged();
    }

    private void GenerateAppointmentTimes()
    {
        AppointmentTimes.Clear();
        var startTime = new DateTime(1, 1, 1, 9, 0, 0);
        var endTime = new DateTime(1, 1, 1, 17, 0, 0);
        var interval = TimeSpan.FromMinutes(20);

        while (startTime < endTime)
        {
            AppointmentTimes.Add(new AppointmentTime
            {
                StartTime = startTime.ToString("HH:mm"),
                EndTime = startTime.Add(interval).ToString("HH:mm"),
                IsSelected = false,
                IsAvailable = random.Next(2) == 0,
            });
            startTime = startTime.Add(interval);
        }

        AvailableSlotCount = AppointmentTimes.Count(e => e.IsAvailable);
    }

    private bool NavigationAllowed(StepNavigationContext context)
    {

        if (IsProcessCanceled)
        {
            return true;
        }
        
        if (context.CurrentStepName == "select_service" && !IsFullFilledService)
        {
            return false;
        }

        if (context.CurrentStepName == "select_appointment" && !IsAppointmentTimeValid)
        {
            return false;
        }

        return true;
    }

    private void OnServiceChange(SelectionItem item)
    {
        if (item.IsSelected)
        {
            ServicesList.ForEach(e => e.IsSelected = false);
        }
        else
        {
            ServicesList.ForEach(e => e.IsSelected = false);
            item.IsSelected = true;
        }

        selectedService.ServiceName = AppointmentSummary.ServiceType = (item.IsSelected ? item.DisplayName : null)!;
        selectedService.Cost = AppointmentSummary.Price = (item.IsSelected ? item.Cost : 0)!;
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

        selectedService.DoctorName = (item.IsSelected ? item.Name : null)!;
        selectedService.DepartmentName = (item.IsSelected ? item.Department : null)!;
        AppointmentSummary.DoctorName = (item.IsSelected ? item.Name : null)!;
        AppointmentSummary.Department = (item.IsSelected ? item.Department : null)!;
        StateHasChanged();
    }

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

    private void SetLoadCount()
    {
        if (screenWidth < 768)
        {
            loadCount = 7;
        }
        else if (screenWidth < 1024)
        {
            loadCount = 14;
        }
        else
        {
            loadCount = 14;
        }
    }

    private void SelectDay(DayItem item)
    {
        if (item.IsSelected)
        {
            DaysList.ForEach(e => e.IsSelected = false);
            AppointmentSummary.AppointmentDate = null;
        }
        else
        {
            DaysList.ForEach(e => e.IsSelected = false);
            item.IsSelected = true;
            AppointmentSummary.AppointmentDate = item.Date.ToShortDateString();
        }

        GenerateAppointmentTimes();
    }

    private void SelectAppointment(AppointmentTime appointment)
    {
        if (!appointment.IsAvailable)
        {
            return;
        }

        if (!appointment.IsSelected)
        {
            AppointmentTimes.ForEach(e => e.IsSelected = false);
            appointment.IsSelected = true;
            AppointmentSummary.AppointmentTime =
                $"{appointment.StartTime.ToString(CultureInfo.CurrentCulture)} - {appointment.EndTime.ToString(CultureInfo.CurrentCulture)}";
            return;
        }

        AppointmentTimes.ForEach(e => e.IsSelected = false);
        AppointmentSummary.AppointmentTime = null;
    }

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

        await stepsRef.NextStep();
        IsFinalResultSuccess = true;
    }

    public class SelectedService
    {
        public string ServiceName { get; set; }
        public string DoctorName { get; set; }
        public string DepartmentName { get; set; }
        public double Cost { get; set; }
    }

    public class DayItem
    {
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AppointmentTime
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool IsSelected { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AppointmentSummaryItem
    {
        public string PatientName { get; set; } = "John Doe";
        public string HospitalName { get; set; } = "XYZ Hospital";
        public string AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string DoctorName { get; set; }

        public string Department { get; set; }
        public string ServiceType { get; set; }
        public double Price { get; set; } = 0.0;
        public string PrimaryColor { get; set; } = "purple";
    }
}