using Volo.Abp.Modularity;

namespace Pusula.Training.HealthCare;

public abstract class HealthCareApplicationTestBase<TStartupModule> : HealthCareTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
