using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Patients;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Users;

namespace Pusula.Training.HealthCare.Handlers
{
    public class PatientDeletedEventHandler(ILogger<PatientDeletedEventHandler> logger) : IDistributedEventHandler<PatientDeletedEto>, ITransientDependency
    {
        public Task HandleEventAsync(PatientDeletedEto eventData)
        {
            logger.LogInformation($" -----> HANDLER -> Patient Number {eventData.PatientNumber}, {eventData.FirstName} {eventData.LastName} was deleted " +
                $" by {eventData.DeletedByUserName} on {eventData.DeletedAt.ToLongTimeString()}.");

            return Task.CompletedTask;
        }
    }
}
