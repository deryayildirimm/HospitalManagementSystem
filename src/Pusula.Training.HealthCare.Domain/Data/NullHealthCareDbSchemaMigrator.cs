using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Pusula.Training.HealthCare.Data;

/* This is used if database provider does't define
 * IHealthCareDbSchemaMigrator implementation.
 */
public class NullHealthCareDbSchemaMigrator : IHealthCareDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
