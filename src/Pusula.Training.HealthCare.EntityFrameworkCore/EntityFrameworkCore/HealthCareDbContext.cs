using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class HealthCareDbContext :
    AbpDbContext<HealthCareDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Protocol> Protocols { get; set; } = null!;
    public DbSet<Patient> Patients { get; set; } = null!;
    public DbSet<MedicalService> Services { get; set; } = null!;
    public DbSet<DepartmentMedicalService> DepartmentMedicalServices { get; set; } = null!;

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }

    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public HealthCareDbContext(DbContextOptions<HealthCareDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */
        if (builder.IsHostDatabase())
        {
            builder.Entity<Patient>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Patients", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.FirstName).HasColumnName(nameof(Patient.FirstName)).IsRequired()
                    .HasMaxLength(PatientConsts.NameMaxLength);
                b.Property(x => x.LastName).HasColumnName(nameof(Patient.LastName)).IsRequired()
                    .HasMaxLength(PatientConsts.LastNameMaxLength);
                b.Property(x => x.MothersName).HasColumnName(nameof(Patient.MothersName))
                    .HasMaxLength(PatientConsts.NameMaxLength);
                b.Property(x => x.FathersName).HasColumnName(nameof(Patient.FathersName))
                    .HasMaxLength(PatientConsts.NameMaxLength);
                b.Property(x => x.IdentityNumber).HasColumnName(nameof(Patient.IdentityNumber))
                    .HasMaxLength(PatientConsts.IdentityNumberLength);
                b.Property(x => x.Nationality).HasColumnName(nameof(Patient.Nationality)).IsRequired();
                b.Property(x => x.PassportNumber).HasColumnName(nameof(Patient.PassportNumber))
                    .HasMaxLength(PatientConsts.PassportNumberMaxLength);
                b.Property(x => x.BirthDate).HasColumnName(nameof(Patient.BirthDate)).IsRequired();
                b.Property(x => x.EmailAddress).HasColumnName(nameof(Patient.EmailAddress))
                    .HasMaxLength(PatientConsts.EmailAddressMaxLength);
                b.Property(x => x.MobilePhoneNumber).HasColumnName(nameof(Patient.MobilePhoneNumber)).IsRequired()
                    .HasMaxLength(PatientConsts.MobilePhoneNumberMaxLength);
                b.Property(x => x.Relative).HasColumnName(nameof(Patient.Relative));
                b.Property(x => x.RelativePhoneNumber).HasColumnName(nameof(Patient.RelativePhoneNumber))
                    .HasMaxLength(PatientConsts.MobilePhoneNumberMaxLength);
                b.Property(x => x.PatientType).HasColumnName(nameof(Patient.PatientType)).IsRequired();
                b.Property(x => x.Address).HasColumnName(nameof(Patient.Address))
                    .HasMaxLength(PatientConsts.AddressMaxLength);
                b.Property(x => x.InsuranceType).HasColumnName(nameof(Patient.InsuranceType)).IsRequired();
                b.Property(x => x.InsuranceNo).HasColumnName(nameof(Patient.InsuranceNo)).IsRequired()
                    .HasMaxLength(PatientConsts.InsuranceNumberMaxLength);
                b.Property(x => x.DiscountGroup).HasColumnName(nameof(Patient.DiscountGroup)).IsRequired();
                b.Property(x => x.Gender).HasColumnName(nameof(Patient.Gender)).IsRequired();
            });

            builder.Entity<Department>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Departments", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(Department.Name)).IsRequired()
                    .HasMaxLength(DepartmentConsts.NameMaxLength);
            });

            builder.Entity<Protocol>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Protocols", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Type).HasColumnName(nameof(Protocol.Type)).IsRequired()
                    .HasMaxLength(ProtocolConsts.TypeMaxLength);
                b.Property(x => x.StartTime).HasColumnName(nameof(Protocol.StartTime));
                b.Property(x => x.EndTime).HasColumnName(nameof(Protocol.EndTime));
                b.HasOne<Patient>().WithMany().IsRequired().HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Department>().WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<MedicalService>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "MedicalServices", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.Name).HasColumnName(nameof(MedicalService.Name)).IsRequired()
                    .HasMaxLength(MedicalServiceConsts.NameMaxLength);

                b.Property(x => x.Cost)
                    .HasColumnName(nameof(MedicalService.Cost))
                    .IsRequired()
                    .HasPrecision(18, 6);

                b.Property(x => x.ServiceCreatedAt).HasColumnName(nameof(MedicalService.ServiceCreatedAt)).IsRequired();
                
                b.HasIndex(e => new { e.Name }).IsUnique();
            });

            builder.Entity<MedicalServicePatient>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "MedicalServicePatients", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.MedicalServiceId, x.PatientId });

                b.HasOne<Patient>()
                    .WithMany(x => x.MedicalServices)
                    .HasForeignKey(x => x.PatientId)
                    .IsRequired();

                b.HasOne<MedicalService>()
                    .WithMany()
                    .HasForeignKey(x => x.MedicalServiceId)
                    .IsRequired();

                b.HasIndex(x => new { x.MedicalServiceId, x.PatientId });
            });
            
            builder.Entity<DepartmentMedicalService>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "DepartmentMedicalServices", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.MedicalServiceId, x.DepartmentId });

                b.HasOne<Department>(sc => sc.Department)
                    .WithMany(x => x.DepartmentMedicalServices)
                    .HasForeignKey(x => x.DepartmentId);

                b.HasOne<MedicalService>(sc => sc.MedicalService)
                    .WithMany(x => x.DepartmentMedicalServices)
                    .HasForeignKey(x => x.MedicalServiceId);

                b.HasIndex(x => new { x.MedicalServiceId, x.DepartmentId }).IsUnique();
            });
        }
    }
}