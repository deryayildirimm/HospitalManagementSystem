using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pusula.Training.HealthCare.Data;
using Volo.Abp.DependencyInjection;

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

public class EntityFrameworkCoreHealthCareDbSchemaMigrator
    : IHealthCareDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreHealthCareDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the HealthCareDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<HealthCareDbContext>()
            .Database
            .MigrateAsync();
    }
}
