using System;
using Volo.Abp.BackgroundJobs;

namespace Pusula.Training.HealthCare.Appointments;

[BackgroundJobName("appointment-track-log")]
public class AppointmentTrackerLogArgs
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}