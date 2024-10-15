using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Patients;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Handlers;

public class PatientViewedEventHandler(ILogger<PatientViewedEventHandler> logger) : IDistributedEventHandler<PatientViewedEto>, ITransientDependency
{
    public Task HandleEventAsync(PatientViewedEto eventData)
    {
        logger.LogInformation($" -----> HANDLER -> Patient {eventData.Id} viewed as {eventData.ViewedAt.ToLongTimeString()}.");

        return Task.CompletedTask;
    }
}
