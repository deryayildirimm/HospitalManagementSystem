using Volo.Abp.Modularity;

namespace Pusula.Training.HealthCare;

[DependsOn(
    typeof(HealthCareApplicationModule),
    typeof(HealthCareDomainTestModule)
)]
public class HealthCareApplicationTestModule : AbpModule
{

}
