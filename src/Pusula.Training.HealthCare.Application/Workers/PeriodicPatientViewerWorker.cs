using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Patients;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Pusula.Training.HealthCare.Workers;

public class PeriodicPatientViewerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public PeriodicPatientViewerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) 
        : base( timer, serviceScopeFactory)
    {
        Timer.Period = 10000;
    }

    [UnitOfWork]
    protected async override Task DoWorkAsync(
        PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogInformation("Starting: PeriodicPatientViewerWorker...");

        var patientRepository = workerContext.ServiceProvider.GetRequiredService<IPatientRepository>();

        var patient = await patientRepository.FirstOrDefaultAsync();

        if (patient != null)
        {
            var backgroundJobManager = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobManager>();

            await backgroundJobManager.EnqueueAsync(new PatientViewLogArgs { Id = patient.Id, Name = $"{patient.FirstName} {patient.LastName}" });
        }

        Logger.LogInformation("Completed: PeriodicPatientViewerWorker...");
    }
}
