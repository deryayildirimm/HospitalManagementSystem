using Medallion.Threading.Redis;
using Medallion.Threading;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.EventBus.RabbitMq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.BackgroundWorkers;
using System.Threading.Tasks;
using Volo.Abp;
using Pusula.Training.HealthCare.Workers;

namespace Pusula.Training.HealthCare;

[DependsOn(
    typeof(HealthCareDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(HealthCareApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpBackgroundJobsRabbitMqModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpBackgroundWorkersModule)
    )]
public class HealthCareApplicationModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<PeriodicPatientViewerWorker>();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "PTH:";
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<HealthCareApplicationModule>();
        });

        context.Services.Replace(ServiceDescriptor.Transient<IBackgroundJobManager, DefaultBackgroundJobManager>());

        Configure<AbpBackgroundJobWorkerOptions>(options =>
        {
            options.DefaultFirstWaitDuration = 10;
            options.DefaultTimeout = 86400;
        });

        var redis = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);

        context.Services
        .AddDataProtection()
        .SetApplicationName("PTH")
            .PersistKeysToStackExchangeRedis(redis, "PTH-Protection-Keys");

        context.Services.AddSingleton<IDistributedLockProvider>(_ => new RedisDistributedSynchronizationProvider(redis.GetDatabase()));
    }
}
