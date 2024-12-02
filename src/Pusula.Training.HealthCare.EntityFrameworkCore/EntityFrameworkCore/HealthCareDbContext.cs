using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.MedicalPersonnel;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Titles;
using Pusula.Training.HealthCare.BloodTests;
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
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.BloodTests.Tests;

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
    public DbSet<Title> Titles { get; set; } = null!;
    public DbSet<Doctor> Doctors { get; set; } = null!;
    public DbSet<DoctorLeave> DoctorLeaves { get; set; } = null!;
    public DbSet<MedicalStaff> MedicalPersonnel { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<District> Districts { get; set; } = null!;
    public DbSet<BloodTest> BloodTests { get; set; } = null!;
    public DbSet<TestCategory> TestCategories { get; set; } = null!;
    public DbSet<Test> Tests { get; set; } = null!;
    public DbSet<BloodTestResult> BloodTestResults { get; set; } = null!;

    public DbSet<Appointment> Appointments { get; set; } = null!;
    public DbSet<AppointmentType> AppointmentTypes { get; set; } = null!;
    public DbSet<DoctorWorkingHour> DoctorWorkingHours { get; set; } = null!;

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
                b.Property(x => x.PatientNumber).HasColumnName(nameof(Patient.PatientNumber)).IsRequired();
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
                
                b.HasMany(x => x.DepartmentMedicalServices)
                    .WithOne(e => e.Department)
                    .HasForeignKey(e => e.DepartmentId);
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
                
                b.Property(x => x.Name)
                    .HasColumnName(nameof(MedicalService.Name))
                    .IsRequired()
                    .HasMaxLength(MedicalServiceConsts.NameMaxLength);

                b.Property(x => x.Cost)
                    .HasColumnName(nameof(MedicalService.Cost))
                    .IsRequired();

                b.Property(x => x.ServiceCreatedAt)
                    .HasColumnName(nameof(MedicalService.ServiceCreatedAt))
                    .IsRequired();
                
                b.HasMany(x => x.DepartmentMedicalServices)
                    .WithOne(e => e.MedicalService)
                    .HasForeignKey(e => e.MedicalServiceId);
            });

            builder.Entity<DepartmentMedicalService>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "DepartmentMedicalServices", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.MedicalServiceId, x.DepartmentId });

                b.HasOne<Department>(sc => sc.Department)
                    .WithMany(x => x.DepartmentMedicalServices)
                    .IsRequired(false)
                    .HasForeignKey(x => x.DepartmentId);

                b.HasOne<MedicalService>(sc => sc.MedicalService)
                    .WithMany(x => x.DepartmentMedicalServices)
                    .IsRequired(false)
                    .HasForeignKey(x => x.MedicalServiceId);
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
                b.Property(x => x.StartDate).HasColumnName(nameof(Doctor.StartDate)).IsRequired();
                b.HasOne<City>().WithMany().IsRequired().HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<District>().WithMany().IsRequired().HasForeignKey(x => x.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Title>().WithMany().IsRequired().HasForeignKey(x => x.TitleId)
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

            builder.Entity<Appointment>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Appointments", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasKey(a => a.Id);

                b.Property(a => a.AppointmentDate)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.AppointmentDate));

                b.Property(a => a.StartTime)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.StartTime));

                b.Property(a => a.EndTime)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.EndTime));

                b.Property(a => a.Status)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.Status))
                    .HasConversion<int>();

                b.Property(a => a.Notes)
                    .HasMaxLength(AppointmentConsts.MaxNotesLength)
                    .HasColumnName(nameof(Appointment.Notes));

                b.Property(a => a.ReminderSent)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.ReminderSent));

                b.Property(a => a.Amount)
                    .IsRequired()
                    .HasColumnName(nameof(Appointment.Amount));

                b.HasOne(a => a.Doctor)
                    .WithMany()
                    .IsRequired()
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);
    
                b.HasOne(a => a.AppointmentType)
                    .WithMany()
                    .IsRequired()
                    .HasForeignKey(a => a.AppointmentTypeId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(a => a.Patient)
                    .WithMany()
                    .IsRequired()
                    .HasForeignKey(a => a.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(a => a.MedicalService)
                    .WithMany()
                    .IsRequired()
                    .HasForeignKey(a => a.MedicalServiceId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<AppointmentType>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "AppointmentTypes", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(AppointmentType.Name)).IsRequired()
                    .HasMaxLength(AppointmentTypeConsts.NameMaxLength);
            });

            builder.Entity<DoctorWorkingHour>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "DoctorWorkingHours", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.Property(x => x.DoctorId).IsRequired().HasColumnName(nameof(DoctorWorkingHour.DoctorId));
                b.Property(x => x.DayOfWeek).IsRequired().HasColumnName(nameof(DoctorWorkingHour.DayOfWeek));
                b.Property(x => x.StartHour).IsRequired().HasColumnName(nameof(DoctorWorkingHour.StartHour));
                b.Property(x => x.EndHour).IsRequired().HasColumnName(nameof(DoctorWorkingHour.EndHour));

                // Unique constraint: Prevent multiple working hours entries for the same doctor on the same day
                b.HasIndex(x => new { x.DoctorId, x.DayOfWeek }).IsUnique();
            });

            builder.Entity<DoctorLeave>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "DoctorLeaves", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.StartDate).HasColumnName(nameof(DoctorLeave.StartDate)).IsRequired();
                b.Property(x => x.EndDate).HasColumnName(nameof(DoctorLeave.EndDate)).IsRequired();
                b.Property(x => x.Reason).HasColumnName(nameof(DoctorLeave.Reason)).HasMaxLength(200);
                b.HasOne<Doctor>().WithMany().IsRequired().HasForeignKey(x => x.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<MedicalStaff>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "MedicalStaff", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.FirstName).HasColumnName(nameof(MedicalStaff.FirstName)).IsRequired()
                    .HasMaxLength(MedicalStaffConsts.FirstNameMaxLength);
                b.Property(x => x.LastName).HasColumnName(nameof(MedicalStaff.LastName)).IsRequired()
                    .HasMaxLength(MedicalStaffConsts.LastNameMaxLength);
                b.Property(x => x.IdentityNumber).HasColumnName(nameof(MedicalStaff.IdentityNumber)).IsRequired()
                    .HasMaxLength(MedicalStaffConsts.IdentityNumberLength);
                b.Property(x => x.BirthDate).HasColumnName(nameof(MedicalStaff.BirthDate)).IsRequired();
                b.Property(x => x.Gender).HasColumnName(nameof(MedicalStaff.Gender)).IsRequired();
                b.Property(x => x.Email).HasColumnName(nameof(MedicalStaff.Email))
                    .HasMaxLength(MedicalStaffConsts.EmailMaxLength);
                b.Property(x => x.PhoneNumber).HasColumnName(nameof(MedicalStaff.PhoneNumber))
                    .HasMaxLength(MedicalStaffConsts.PhoneNumberMaxLength);
                b.Property(x => x.StartDate).HasColumnName(nameof(MedicalStaff.StartDate)).IsRequired();
                b.HasOne<City>().WithMany().IsRequired().HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<District>().WithMany().IsRequired().HasForeignKey(x => x.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Department>().WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<City>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Cities", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(City.Name)).IsRequired()
                    .HasMaxLength(CityConsts.NameMaxLength);
            });

            builder.Entity<District>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Districts", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(District.Name)).IsRequired()
                    .HasMaxLength(DistrictConsts.NameMaxLength);
                b.HasOne<City>().WithMany().IsRequired().HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<BloodTest>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTests", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Status).HasColumnName(nameof(BloodTest.Status)).IsRequired();
                b.Property(x => x.DateCreated).HasColumnName(nameof(BloodTest.DateCreated));
                b.Property(x => x.DateCompleted).HasColumnName(nameof(BloodTest.DateCompleted));
                b.HasOne<Doctor>().WithMany().IsRequired().HasForeignKey(x => x.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Patient>().WithMany().IsRequired().HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<TestCategory>().WithMany().IsRequired().HasForeignKey(x => x.TestCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<TestCategory>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "TestCategories", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(TestCategory.Name)).IsRequired()
                    .HasMaxLength(BloodTestConst.CategoryNameMax);
                b.Property(x => x.Description).HasColumnName(nameof(TestCategory.Description)).IsRequired()
                    .HasMaxLength(BloodTestConst.CategoryDescriptionMax);
                b.Property(x => x.Url).HasColumnName(nameof(TestCategory.Url)).IsRequired();
                b.Property(x => x.Price).HasColumnName(nameof(TestCategory.Price)).IsRequired();
            });

            builder.Entity<Test>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Tests", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(TestCategory.Name)).IsRequired()
                    .HasMaxLength(BloodTestConst.TestNameMax);
                b.Property(x => x.MinValue).HasColumnName(nameof(Test.MinValue)).IsRequired();
                b.Property(x => x.MaxValue).HasColumnName(nameof(Test.MaxValue)).IsRequired();
                b.HasOne<TestCategory>().WithMany().IsRequired().HasForeignKey(x => x.TestCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<BloodTestResult>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTestResults", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Value).HasColumnName(nameof(BloodTestResult.Value)).IsRequired();
                b.Property(x => x.BloodResultStatus).HasColumnName(nameof(BloodTestResult.BloodResultStatus))
                    .IsRequired();
                b.HasOne<BloodTest>().WithMany().IsRequired().HasForeignKey(x => x.BloodTestId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne<Test>().WithMany().IsRequired().HasForeignKey(x => x.TestId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
        }
    }
}