using Volo.Abp.Modularity;

namespace Pusula.Training.HealthCare;

[DependsOn(
    typeof(HealthCareDomainModule),
    typeof(HealthCareTestBaseModule)
)]
public class HealthCareDomainTestModule : AbpModule
{

}
