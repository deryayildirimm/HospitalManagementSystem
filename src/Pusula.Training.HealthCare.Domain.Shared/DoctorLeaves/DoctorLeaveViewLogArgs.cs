using System;
using Volo.Abp.BackgroundJobs;

namespace Pusula.Training.HealthCare.DoctorLeaves;

[BackgroundJobName("doctorLeave-view-log")]
public class DoctorLeaveViewLogArgs
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}