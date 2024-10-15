using Pusula.Training.HealthCare.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Pusula.Training.HealthCare.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(HealthCareEntityFrameworkCoreModule),
    typeof(HealthCareApplicationContractsModule)
    )]
public class HealthCareDbMigratorModule : AbpModule
{
}
