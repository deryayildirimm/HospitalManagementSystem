using System;
using Volo.Abp.BackgroundJobs;

namespace Pusula.Training.HealthCare.Patients;

[BackgroundJobName("patient-view-log")]
public class PatientViewLogArgs
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
