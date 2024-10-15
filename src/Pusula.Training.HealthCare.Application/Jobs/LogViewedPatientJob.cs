using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Patients;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Pusula.Training.HealthCare.Jobs;

public class LogViewedPatientJob() : AsyncBackgroundJob<PatientViewLogArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(PatientViewLogArgs args)
    {
        Logger.LogInformation($" -----> BACKGROUND-JOB -> {args.Name} with Id {args.Id} viewed.");

        await Task.CompletedTask;
    }
}
