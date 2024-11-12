using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Titles;
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
    public DbSet<Title> Titles { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;


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

            builder.Entity<Title>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Titles", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.TitleName).HasColumnName(nameof(Title.TitleName)).IsRequired()
                    .HasMaxLength(TitleConsts.TitleNameMaxLength);
            });

            builder.Entity<Doctor>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Doctors", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.FirstName).HasColumnName(nameof(Doctor.FirstName)).IsRequired()
                    .HasMaxLength(DoctorConsts.FirstNameMaxLength);
                b.Property(x => x.LastName).HasColumnName(nameof(Doctor.LastName)).IsRequired()
                    .HasMaxLength(DoctorConsts.LastNameMaxLength);
                b.Property(x => x.IdentityNumber).HasColumnName(nameof(Doctor.IdentityNumber)).IsRequired()
                    .HasMaxLength(DoctorConsts.IdentityNumberLength);
                b.Property(x => x.BirthDate).HasColumnName(nameof(Doctor.BirthDate)).IsRequired();
                b.Property(x => x.Gender).HasColumnName(nameof(Doctor.Gender)).IsRequired();
                b.Property(x => x.Email).HasColumnName(nameof(Doctor.Email))
                    .HasMaxLength(DoctorConsts.EmailMaxLength);
                b.Property(x => x.PhoneNumber).HasColumnName(nameof(Doctor.PhoneNumber))
                    .HasMaxLength(DoctorConsts.PhoneNumberMaxLength);
                b.Property(x => x.YearOfExperience).HasColumnName(nameof(Doctor.YearOfExperience));
                b.Property(x => x.City).HasColumnName(nameof(Doctor.City)).IsRequired()
                    .HasMaxLength(DoctorConsts.CityMaxLength);
                b.Property(x => x.District).HasColumnName(nameof(Doctor.District)).IsRequired()
                    .HasMaxLength(DoctorConsts.DistrictMaxLength);
                b.HasOne<Title>().WithMany().IsRequired().HasForeignKey(x => x.TitleId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Department>().WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(HealthCareConsts.DbTablePrefix + "YourEntities", HealthCareConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
    }
}