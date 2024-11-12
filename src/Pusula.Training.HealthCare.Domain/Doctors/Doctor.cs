using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Doctors;

public class Doctor : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string FirstName { get; set; }
    [NotNull]
    public virtual string LastName { get; set; }
    [NotNull]
    public virtual string IdentityNumber { get; set; }
    [NotNull]
    public virtual DateTime BirthDate { get; set; }
    [NotNull]
    public virtual EnumGender Gender { get; set; }
    [CanBeNull]
    public virtual string? Email { get; set; }
    [CanBeNull]
    public virtual string? PhoneNumber { get; set; }
    [NotNull]
    public virtual int YearOfExperience { get; set; }
    [NotNull]
    public virtual string City { get; set; }
    [NotNull]
    public virtual string District { get; set; }
    public virtual Guid DepartmentId { get; set; }
    public virtual Guid TitleId { get; set; }

    protected Doctor()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        IdentityNumber = string.Empty;
        BirthDate = DateTime.Now;
        Gender = EnumGender.OTHER;
        YearOfExperience = 0;
        City = string.Empty;
        District = string.Empty;
        DepartmentId = Guid.Empty;
        TitleId = Guid.Empty;
    }

    public Doctor(Guid id, Guid titleId, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender,
        int yearOfExperience, string city, string district, string? email = null, string? phoneNumber = null )
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), DoctorConsts.FirstNameMaxLength, DoctorConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), DoctorConsts.LastNameMaxLength, DoctorConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), DoctorConsts.IdentityNumberLength, DoctorConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), DoctorConsts.GenderMinValue, DoctorConsts.GenderMaxValue);
        Check.Range(yearOfExperience, nameof(yearOfExperience), 0, 100);
        Check.NotNullOrWhiteSpace(city, nameof(city), DoctorConsts.CityMaxLength, DoctorConsts.CityMinLength);
        Check.NotNullOrWhiteSpace(district, nameof(district), DoctorConsts.DistrictMaxLength, DoctorConsts.DistrictMinLength);
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        
        
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Gender = gender;
        YearOfExperience = yearOfExperience;
        City = city;
        District = district;
        Email = email;
        PhoneNumber = phoneNumber;
        TitleId = titleId;
        DepartmentId = departmentId;
    }
}