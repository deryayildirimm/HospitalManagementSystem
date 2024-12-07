using Microsoft.Extensions.Logging;
using Pusula.Training.HealthCare.Insurances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Handlers
{
    public class InsuranceCreatedEventHandler(ILogger<InsuranceCreatedEventHandler> logger) : IDistributedEventHandler<InsuranceCreatedEto>, ITransientDependency
    {
        public Task HandleEventAsync(InsuranceCreatedEto eventData)
        {
            logger.LogInformation(
                $" -----> HANDLER -> The insurance policy with policy number {eventData.PolicyNumber} from the company {eventData.InsuranceCompanyName} was created on {eventData.CreatedDate}."
            );

            return Task.CompletedTask;
        }
    }
}
