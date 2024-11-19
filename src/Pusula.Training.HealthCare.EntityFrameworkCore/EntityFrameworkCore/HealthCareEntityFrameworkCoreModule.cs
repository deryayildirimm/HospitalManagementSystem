using System;
using Microsoft.Extensions.DependencyInjection;
using Pusula.Training.HealthCare.Cities;
using Volo.Abp.Uow;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Titles;

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

[DependsOn(
    typeof(HealthCareDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule),
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule)
    )]
public class HealthCareEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        HealthCareEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<HealthCareDbContext>(options =>
        {
                /* Remove "includeAllEntities: true" to create
                 * default repositories only for aggregate roots */
            options.AddDefaultRepositories(includeAllEntities: true);

            options.AddRepository<Patient, EfCorePatientRepository>();
            options.AddRepository<Protocol, EfCoreProtocolRepository>();
            options.AddRepository<Department, EfCoreDepartmentRepository>();
            options.AddRepository<Title, EfCoreTitleRepository>();
            options.AddRepository<Doctor, EfCoreDoctorRepository>();
            options.AddRepository<City, EfCoreCityRepository>();
            options.AddRepository<District, EfCoreDistrictRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
                /* The main point to change your DBMS.
                 * See also HealthCareMigrationsDbContextFactory for EF Core tooling. */
            options.UseNpgsql();
        });

    }
}
