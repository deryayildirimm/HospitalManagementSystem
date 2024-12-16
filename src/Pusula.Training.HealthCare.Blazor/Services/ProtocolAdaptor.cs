using Pusula.Training.HealthCare.Protocols;
using Microsoft.AspNetCore.Components;
using Volo.Abp.DependencyInjection;
namespace Pusula.Training.HealthCare.Blazor.Services
{
    public class ProtocolAdaptor : GenericAdaptor<IProtocolsAppService, GetProtocolsInput, ProtocolDto>
    {
        public ProtocolAdaptor(IProtocolsAppService service) : base(service)
        {

        }
    }
}
