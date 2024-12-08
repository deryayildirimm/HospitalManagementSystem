using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Appointments;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Pusula.Training.HealthCare.Jobs;

public class LogAppointmentTrackerJob() : AsyncBackgroundJob<AppointmentTrackerLogArgs>, ITransientDependency
{
    public override async Task ExecuteAsync(AppointmentTrackerLogArgs args)
    {
        Logger.LogInformation($" -----> APPOINTMENT-BACKGROUND-JOB -> {args.Name} with Id {args.Id}.");

        await Task.CompletedTask;
    }
}