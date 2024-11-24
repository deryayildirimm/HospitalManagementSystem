using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorDashboard : ComponentBase
{
    private DateTime CurrentDate { get; set; }

    private List<AppointmentData> DataSource { get; set; }

    private GetAppointmentsWithNavigationPropertiesInput AppointmentsFilter { get; set; }
    
    private Guid DoctorId { get; set; }
    
    private DoctorWithNavigationPropertiesDto DoctorWithNavigation { get; set; }
    private string DoctorNameInfo { get; set; }

    #region FilterData

    private int AppointmentPageSize { get; set; } = 20;
    private int AppointmentCurrentPage { get; set; } = 1;
    #endregion

    public DoctorDashboard()
    {
        
        DoctorId = Guid.Parse("3a166c0c-bea0-a560-d3f0-4b6f3271c922");
        DoctorWithNavigation = new DoctorWithNavigationPropertiesDto();
        CurrentDate = DateTime.Now;
        AppointmentsFilter = new GetAppointmentsWithNavigationPropertiesInput
        {
            DoctorId = DoctorId,
            AppointmentMinDate = CurrentDate,
            AppointmentMaxDate = CurrentDate.AddDays(7),
            MaxResultCount = AppointmentPageSize,
            SkipCount = (AppointmentCurrentPage - 1) * AppointmentPageSize,
        };
        DataSource = [];
        DoctorNameInfo = "";
    }

    protected override async Task OnInitializedAsync()
    {
        await GetDoctor();
        await GetAppointments();
    }

    private async Task GetDoctor()
    {
        try
        {
            DoctorWithNavigation = await DoctorAppService.GetWithNavigationPropertiesAsync(DoctorId);
            DoctorNameInfo =
                $"{DoctorWithNavigation.Title.TitleName} {DoctorWithNavigation.Doctor.FirstName} {DoctorWithNavigation.Doctor.LastName}";
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
            
            var items = ((await AppointmentAppService.GetListWithNavigationPropertiesAsync(AppointmentsFilter)).Items).ToList();
            if (items.Count > 0)
            {
                DataSource = items.Select(x => new AppointmentData
                {
                    Id = x.Appointment.Id,
                    PatientName = x.Patient.FirstName + " " + x.Patient.LastName,
                    DoctorName = x.Doctor.FirstName + " " + x.Doctor.LastName,
                    StartTime = x.Appointment.StartTime,
                    EndTime = x.Appointment.EndTime,
                    ServiceName = x.MedicalService.Name
                }).ToList();

            }
        }
        catch (Exception e)
        {
            DataSource = [];
            throw new UserFriendlyException(e.Message);
        }
    }
    
    
    public class AppointmentData
    {
        public Guid Id { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime DateOnly { get; set; }
        public DateTime HourOnly { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsAllDay { get; set; }
    }
}