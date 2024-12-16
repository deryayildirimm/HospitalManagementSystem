using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Blazor.Models;
using Pusula.Training.HealthCare.Doctors;
using Syncfusion.Blazor.Schedule.Internal;
using Volo.Abp;

namespace Pusula.Training.HealthCare.Blazor.Components.Pages;

public partial class DoctorDashboard : ComponentBase
{
    private DateTime CurrentDate { get; set; }

    private List<AppointmentCustomData> DataSource { get; set; }

    private GetAppointmentsInput AppointmentsFilter { get; set; }

    private DoctorWithNavigationPropertiesDto DoctorWithNavigation { get; set; }
    private GetDoctorsInput DoctorsInput { get; set; }
    private string DoctorNameInfo { get; set; }

    #region FilterData

    private int AppointmentPageSize { get; set; } = 50;
    private int AppointmentCurrentPage { get; set; } = 1;

    #endregion

    public DoctorDashboard()
    {
        DoctorWithNavigation = new DoctorWithNavigationPropertiesDto();
        CurrentDate = DateTime.Now;
        AppointmentsFilter = new GetAppointmentsInput
        {
            AppointmentMinDate = CurrentDate,
            AppointmentMaxDate = CurrentDate.AddDays(7),
            MaxResultCount = AppointmentPageSize,
            SkipCount = (AppointmentCurrentPage - 1) * AppointmentPageSize,
        };
        DoctorsInput = new GetDoctorsInput
        {
            MaxResultCount = 1
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
            var doctors = (await DoctorAppService.GetListAsync(DoctorsInput)).Items;

            if (doctors.Any())
            {
                DoctorWithNavigation = doctors[0];
                DoctorNameInfo =
                    $"{DoctorWithNavigation.Title.TitleName} {DoctorWithNavigation.Doctor.FirstName} {DoctorWithNavigation.Doctor.LastName}";

                AppointmentsFilter.DoctorId = DoctorWithNavigation.Doctor.Id;
            }
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
            
            if(items.Count == 0)
            {
                DataSource = [];
                return;
            }

            DataSource = items.Select(x => new AppointmentCustomData
            {
                Id = x.Id,
                PatientName = $"{x.Patient.FirstName} {x.Patient.LastName}",
                DoctorName = $"{x.Doctor.FirstName} {x.Doctor.LastName}",
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                ServiceName = x.MedicalService.Name
            }).ToList();
        }
        catch (Exception e)
        {
            DataSource = [];
            await UiMessageService.Error(e.Message);
        }
    }
}