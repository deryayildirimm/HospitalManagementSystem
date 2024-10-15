using Volo.Abp.Modularity;

namespace Pusula.Training.HealthCare;

/* Inherit from this class for your domain layer tests. */
public abstract class HealthCareDomainTestBase<TStartupModule> : HealthCareTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
