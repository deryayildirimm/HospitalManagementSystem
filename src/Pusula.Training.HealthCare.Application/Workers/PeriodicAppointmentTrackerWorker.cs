using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Pusula.Training.HealthCare.Workers;

public class PeriodicAppointmentTrackerWorker : AsyncPeriodicBackgroundWorkerBase
{
    public PeriodicAppointmentTrackerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        Timer.Period = 1000000;
    }

    [UnitOfWork]
    protected override async Task DoWorkAsync(
        PeriodicBackgroundWorkerContext workerContext)
    {
        Logger.LogInformation("Starting: PeriodicAppointmentTrackerWorker...");

        var appointmentRepository = workerContext.ServiceProvider.GetRequiredService<IAppointmentRepository>();

        var appointment = await appointmentRepository.FirstOrDefaultAsync();

        if (appointment != null)
        {
            var backgroundJobManager = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobManager>();

            await backgroundJobManager.EnqueueAsync(new AppointmentTrackerLogArgs
                { Id = appointment.Id, Name = $"Appointment Date:{appointment.AppointmentDate}, Status: {appointment.Status}" });
        }

        Logger.LogInformation("Completed: PeriodicAppointmentTrackerWorker...");
    }
}