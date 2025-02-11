﻿using Microsoft.EntityFrameworkCore;
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
using Pusula.Training.HealthCare.Treatment.Examinations;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Pusula.Training.HealthCare.Treatment.Icds;
using Pusula.Training.HealthCare.ProtocolTypes;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.Restrictions;
using Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;
using Pusula.Training.HealthCare.BloodTests.Reports;
using Pusula.Training.HealthCare.BloodTests.Results;

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
    public DbSet<ProtocolType> ProtocolTypes { get; set; } = null!;
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
    public DbSet<BloodTestReport> BloodTestReports { get; set; } = null!;

    public DbSet<Insurance> Insurances { get; set; } = null!;
    public DbSet<Restriction> Restrictions { get; set; } = null!;

    // Treatment
    public DbSet<Icd> Icds { get; set; } = null!;
    public DbSet<Examination> Examinations { get; set; } = null!;
    public DbSet<FamilyHistory> FamilyHistories { get; set; } = null!;
    public DbSet<Background> Backgrounds { get; set; } = null!;
    public DbSet<PhysicalFinding> PhysicalFindings { get; set; } = null!;

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
                    .HasMaxLength(PatientConsts.IdentityNumberLength).IsRequired();
                b.Property(x => x.PassportNumber).HasColumnName(nameof(Patient.PassportNumber))
                    .HasMaxLength(PatientConsts.PassportNumberMaxLength);
                b.Property(x => x.Nationality).HasColumnName(nameof(Patient.Nationality));
                b.Property(x => x.BirthDate).HasColumnName(nameof(Patient.BirthDate)).IsRequired();
                b.Property(x => x.EmailAddress).HasColumnName(nameof(Patient.EmailAddress))
                    .HasMaxLength(PatientConsts.EmailAddressMaxLength);
                b.Property(x => x.MobilePhoneNumber).HasColumnName(nameof(Patient.MobilePhoneNumber))
                    .HasMaxLength(PatientConsts.MobilePhoneNumberMaxLength);
                b.Property(x => x.Relative).HasColumnName(nameof(Patient.Relative));
                b.Property(x => x.RelativePhoneNumber).HasColumnName(nameof(Patient.RelativePhoneNumber))
                    .HasMaxLength(PatientConsts.MobilePhoneNumberMaxLength);
                b.Property(x => x.PatientType).HasColumnName(nameof(Patient.PatientType));
                b.Property(x => x.Address).HasColumnName(nameof(Patient.Address))
                    .HasMaxLength(PatientConsts.AddressMaxLength);
                b.Property(x => x.DiscountGroup).HasColumnName(nameof(Patient.DiscountGroup));
                b.Property(x => x.Gender).HasColumnName(nameof(Patient.Gender));
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

                b.HasMany(x => x.Doctors)
                    .WithOne(d => d.Department)
                    .HasForeignKey(d => d.DepartmentId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Protocol>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Protocols", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Note).HasColumnName(nameof(Protocol.Note))
                    .HasMaxLength(ProtocolConsts.MaxNotesLength);
                b.Property(x => x.StartTime).HasColumnName(nameof(Protocol.StartTime)).IsRequired();
                b.Property(x => x.EndTime).HasColumnName(nameof(Protocol.EndTime));
                b.HasOne(p => p.Patient).WithMany().IsRequired().HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(p => p.ProtocolType).WithMany().IsRequired().HasForeignKey(o => o.ProtocolTypeId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(p => p.Department).WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(p => p.Doctor).WithMany().IsRequired().HasForeignKey(p => p.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(p => p.Insurance).WithMany().IsRequired().HasForeignKey(p => p.InsuranceId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasMany(x => x.ProtocolMedicalServices)
                    .WithOne(e => e.Protocol)
                    .HasForeignKey(e => e.ProtocolId);
            });

            builder.Entity<ProtocolType>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "ProtocolTypes", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(ProtocolType.Name)).IsRequired()
                    .HasMaxLength(ProtocolTypeConsts.NameMaxLength);
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

                b.Property(x => x.ServiceCreatedAt).HasColumnName(nameof(MedicalService.ServiceCreatedAt)).IsRequired();

                b.HasMany(x => x.DepartmentMedicalServices)
                    .WithOne(e => e.MedicalService)
                    .HasForeignKey(e => e.MedicalServiceId);

                b.HasMany(x => x.ProtocolMedicalServices)
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

            builder.Entity<ProtocolMedicalService>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "ProtocolMedicalServices", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.MedicalServiceId, x.ProtocolId });

                b.HasOne<Protocol>(sc => sc.Protocol)
                    .WithMany(x => x.ProtocolMedicalServices)
                    .IsRequired(false)
                    .HasForeignKey(x => x.ProtocolId);

                b.HasOne<MedicalService>(sc => sc.MedicalService)
                    .WithMany(x => x.ProtocolMedicalServices)
                    .IsRequired(false)
                    .HasForeignKey(x => x.MedicalServiceId);

                b.HasIndex(x => new { x.MedicalServiceId, x.ProtocolId });
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
                b.HasOne(d => d.City).WithMany().IsRequired().HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(d => d.District).WithMany().IsRequired().HasForeignKey(x => x.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(d => d.Title).WithMany().IsRequired().HasForeignKey(x => x.TitleId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(x => x.Department)
                    .WithMany(d => d.Doctors)
                    .HasForeignKey(x => x.DepartmentId)
                    .IsRequired(false)
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

                b.Property(a => a.CancellationNotes)
                    .IsRequired(false)
                    .HasColumnName(nameof(Appointment.CancellationNotes));

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

                b.HasOne(a => a.Department)
                    .WithMany()
                    .IsRequired()
                    .HasForeignKey(a => a.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<AppointmentType>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "AppointmentTypes", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(AppointmentType.Name)).IsRequired()
                    .HasMaxLength(AppointmentTypeConsts.NameMaxLength);
            });

            builder.Entity<Restriction>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Restrictions", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasKey(a => a.Id);

                b.Property(a => a.MinAge)
                    .IsRequired(false)
                    .HasColumnName(nameof(Restriction.MinAge));

                b.Property(a => a.MaxAge)
                    .IsRequired(false)
                    .HasColumnName(nameof(Restriction.MaxAge));

                b.Property(a => a.AllowedGender)
                    .IsRequired()
                    .HasColumnName(nameof(Restriction.AllowedGender));

                b.HasOne(a => a.Doctor)
                    .WithMany()
                    .IsRequired(false)
                    .HasForeignKey(a => a.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(a => a.Department)
                    .WithMany()
                    .IsRequired(false)
                    .HasForeignKey(a => a.DepartmentId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne(a => a.MedicalService)
                    .WithMany()
                    .IsRequired(false)
                    .HasForeignKey(a => a.MedicalServiceId)
                    .OnDelete(DeleteBehavior.NoAction);
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
                b.Property(x => x.LeaveType).HasColumnName(nameof(DoctorLeave.LeaveType)).IsRequired();
                b.Property(x => x.Reason).HasColumnName(nameof(DoctorLeave.Reason)).HasMaxLength(200);
                b.HasOne(x => x.Doctor)
                    .WithMany()
                    .HasForeignKey(x => x.DoctorId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.NoAction);
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
                b.HasOne(m => m.City).WithMany().IsRequired().HasForeignKey(x => x.CityId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(m => m.District).WithMany().IsRequired().HasForeignKey(x => x.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(m => m.Department).WithMany().IsRequired().HasForeignKey(x => x.DepartmentId)
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
                b.HasOne(x => x.Doctor).WithMany().IsRequired(false).HasForeignKey(x => x.DoctorId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(x => x.Patient).WithMany().IsRequired(false).HasForeignKey(x => x.PatientId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasMany(x => x.BloodTestCategories).WithOne().IsRequired().HasForeignKey(x => x.BloodTestId)
                    .OnDelete(DeleteBehavior.NoAction);
                b.HasOne(d => d.BloodTestReport).WithOne(d => d.BloodTest).IsRequired().HasForeignKey<BloodTestReport>(d => d.BloodTestId)
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

            builder.Entity<BloodTestCategory>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTestCategories", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.BloodTestId, x.TestCategoryId });

                b.HasOne(x => x.BloodTest).WithMany(x => x.BloodTestCategories).IsRequired().HasForeignKey(x => x.BloodTestId);
                b.HasOne(x => x.TestCategory).WithMany().IsRequired(false).HasForeignKey(x => x.TestCategoryId);                
            });

            builder.Entity<BloodTestReport>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTestReports", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.HasMany(x => x.Results).WithOne().IsRequired().HasForeignKey(x => x.BloodTestReportId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<BloodTestResult>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTestResults", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Value).HasColumnName(nameof(BloodTestResult.Value)).IsRequired();
                b.Property(x => x.BloodResultStatus).HasColumnName(nameof(BloodTestResult.BloodResultStatus))
                    .IsRequired();
                b.HasOne(x => x.Test).WithMany().IsRequired(false).HasForeignKey(x => x.TestId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<BloodTestReportResult>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "BloodTestReportResults", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.BloodTestReportId, x.BloodTestResultId });

                b.HasOne(x => x.BloodTestReport).WithMany(x => x.Results).IsRequired(false).HasForeignKey(x => x.BloodTestReportId);
                b.HasOne(x => x.BloodTestResult).WithMany().IsRequired(false).HasForeignKey(x => x.BloodTestResultId);
            });

            builder.Entity<Test>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Tests", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Name).HasColumnName(nameof(TestCategory.Name)).IsRequired()
                    .HasMaxLength(BloodTestConst.TestNameMax);
                b.Property(x => x.MinValue).HasColumnName(nameof(Test.MinValue)).IsRequired();
                b.Property(x => x.MaxValue).HasColumnName(nameof(Test.MaxValue)).IsRequired();
                b.HasOne(x => x.TestCategory).WithMany().IsRequired().HasForeignKey(x => x.TestCategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Insurance>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Insurances", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.PolicyNumber).HasColumnName(nameof(Insurance.PolicyNumber)).IsRequired();
                b.Property(x => x.PremiumAmount).HasColumnName(nameof(Insurance.PremiumAmount));
                b.Property(x => x.CoverageAmount).HasColumnName(nameof(Insurance.CoverageAmount));
                b.Property(x => x.StartDate).HasColumnName(nameof(Insurance.StartDate));
                b.Property(x => x.EndDate).HasColumnName(nameof(Insurance.EndDate));
                b.Property(x => x.InsuranceCompanyName).HasColumnName(nameof(Insurance.InsuranceCompanyName))
                    .IsRequired();
                b.Property(x => x.Description).HasColumnName(nameof(Insurance.Description));
            });


            builder.Entity<Icd>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Icds", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.CodeNumber).HasColumnName(nameof(Icd.CodeNumber)).IsRequired()
                    .HasMaxLength(IcdConsts.CodeNumberMaxLength);
                b.Property(x => x.Detail).HasColumnName(nameof(Icd.Detail)).IsRequired()
                    .HasMaxLength(IcdConsts.DetailMaxLength);
            });

            builder.Entity<Examination>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Examinations", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Date).HasColumnName(nameof(Examination.Date)).IsRequired();
                b.Property(x => x.Complaint).HasColumnName(nameof(Examination.Complaint)).IsRequired()
                    .HasMaxLength(ExaminationConsts.ComplaintMaxLength);
                b.Property(x => x.StartDate).HasColumnName(nameof(Examination.StartDate));
                b.Property(x => x.Story).HasColumnName(nameof(Examination.Story))
                    .HasMaxLength(ExaminationConsts.StoryMaxLength);
                b.HasOne(d => d.Background)
                    .WithOne(d => d.Examination)
                    .IsRequired()
                    .HasForeignKey<Background>(d => d.ExaminationId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(d => d.FamilyHistory)
                    .WithOne(d => d.Examination)
                    .IsRequired()
                    .HasForeignKey<FamilyHistory>(d => d.ExaminationId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(d => d.PhysicalFinding)
                    .WithOne(d => d.Examination)
                    .IsRequired()
                    .HasForeignKey<PhysicalFinding>(d => d.ExaminationId)
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(d => d.Protocol)
                    .WithOne()
                    .IsRequired()
                    .HasForeignKey<Examination>(x => x.ProtocolId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ExaminationIcd>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "ExaminationIcds", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();

                b.HasKey(x => new { x.ExaminationId, x.IcdId });

                b.HasOne(ei => ei.Examination)
                    .WithMany(e => e.ExaminationIcds)
                    .IsRequired(false)
                    .HasForeignKey(ei => ei.ExaminationId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(ei => ei.Icd)
                    .WithMany()
                    .IsRequired(false)
                    .HasForeignKey(ei => ei.IcdId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<FamilyHistory>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "FamilyHistories", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.MotherDisease).HasColumnName(nameof(FamilyHistory.MotherDisease))
                    .HasMaxLength(FamilyHistoryConsts.DiseaseMaxLength);
                b.Property(x => x.FatherDisease).HasColumnName(nameof(FamilyHistory.FatherDisease))
                    .HasMaxLength(FamilyHistoryConsts.DiseaseMaxLength);
                b.Property(x => x.SisterDisease).HasColumnName(nameof(FamilyHistory.SisterDisease))
                    .HasMaxLength(FamilyHistoryConsts.DiseaseMaxLength);
                b.Property(x => x.BrotherDisease).HasColumnName(nameof(FamilyHistory.BrotherDisease))
                    .HasMaxLength(FamilyHistoryConsts.DiseaseMaxLength);
                b.Property(x => x.AreParentsRelated).IsRequired();
            });

            builder.Entity<Background>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "Backgrounds", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Allergies).HasColumnName(nameof(Background.Allergies))
                    .HasMaxLength(BackgroundConsts.AllergiesMaxLength);
                b.Property(x => x.Medications).HasColumnName(nameof(Background.Medications))
                    .HasMaxLength(BackgroundConsts.MedicationsMaxLength);
                b.Property(x => x.Habits).HasColumnName(nameof(Background.Habits))
                    .HasMaxLength(BackgroundConsts.HabitsMaxLength);
            });
            
            builder.Entity<PhysicalFinding>(b =>
            {
                b.ToTable(HealthCareConsts.DbTablePrefix + "PhysicalFindings", HealthCareConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.Weight).HasColumnName(nameof(PhysicalFinding.Weight));
                b.Property(x => x.Height).HasColumnName(nameof(PhysicalFinding.Height));
                b.Property(x => x.BodyTemperature).HasColumnName(nameof(PhysicalFinding.BodyTemperature));
                b.Property(x => x.Pulse).HasColumnName(nameof(PhysicalFinding.Pulse));
                b.Property(x => x.Vki).HasColumnName(nameof(PhysicalFinding.Vki));
                b.Property(x => x.Vya).HasColumnName(nameof(PhysicalFinding.Vya));
                b.Property(x => x.Kbs).HasColumnName(nameof(PhysicalFinding.Kbs));
                b.Property(x => x.Kbd).HasColumnName(nameof(PhysicalFinding.Kbd));
                b.Property(x => x.Spo2).HasColumnName(nameof(PhysicalFinding.Spo2));
            });
        }
    }
}