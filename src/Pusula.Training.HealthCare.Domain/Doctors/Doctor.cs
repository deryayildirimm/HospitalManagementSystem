using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Doctors;

public class Doctor : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string FirstName { get; protected set; }
    [NotNull]
    public virtual string LastName { get; protected set; }
    [NotNull]
    public virtual string IdentityNumber { get; protected set; }
    [NotNull]
    public virtual DateTime BirthDate { get; protected set; }
    [NotNull]
    public virtual EnumGender Gender { get; protected set; }
    [CanBeNull]
    public virtual string? Email { get; protected set; }
    [CanBeNull]
    public virtual string? PhoneNumber { get; protected set; }
    [NotNull]
    public virtual DateTime StartDate { get; protected set; }
    [NotNull]
    public virtual Guid CityId { get; protected set; }
    public virtual City City { get; protected set; }
    [NotNull]
    public virtual Guid DistrictId { get; protected set; }
    public virtual District District { get; protected set; }
    [NotNull]
    public virtual Guid DepartmentId { get; protected set; }
    public virtual Department Department { get; protected set; }
    [NotNull]
    public virtual Guid TitleId { get; protected set; }
    public virtual Title Title { get; protected set; }

    protected Doctor()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        IdentityNumber = string.Empty;
        BirthDate = DateTime.Now;
        Gender = EnumGender.OTHER;
        StartDate = DateTime.Now;
        CityId = Guid.Empty;
        DistrictId = Guid.Empty;
        DepartmentId = Guid.Empty;
        TitleId = Guid.Empty;
    }

    public Doctor(Guid id, Guid cityId, Guid districtId, Guid titleId, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender,
        DateTime startDate, string? email = null, string? phoneNumber = null )
    {
        Id = id;
        SetFirstName(firstName);
        SetLastName(lastName);
        SetIdentityNumber(identityNumber);
        SetBirthDate(birthDate);
        SetGender(gender);
        SetStartDate(startDate);
        SetCityId(cityId);
        SetDistrictId(districtId);
        SetEmail(email);
        SetPhoneNumber(phoneNumber);
        SetTitleId(titleId);
        SetDepartmentId(departmentId);
    }

    public void SetFirstName(string firstName)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), DoctorConsts.FirstNameMaxLength, DoctorConsts.FirstNameMinLength);
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), DoctorConsts.LastNameMaxLength, DoctorConsts.LastNameMinLength);
        LastName = lastName;
    }

    public void SetIdentityNumber(string identityNumber)
    {
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), DoctorConsts.IdentityNumberLength, DoctorConsts.IdentityNumberLength);
        IdentityNumber = identityNumber;
    }

    public void SetBirthDate(DateTime birthDate)
    {
        BirthDate = birthDate;
    }

    public void SetGender(EnumGender gender)
    {
        Check.Range((int)gender, nameof(gender), DoctorConsts.GenderMinValue, DoctorConsts.GenderMaxValue);
        Gender = gender;
    }

    public void SetStartDate(DateTime startDate) => StartDate = startDate;
    public void SetEmail(string? email) => Email = email;
    public void SetPhoneNumber(string? phoneNumber) => PhoneNumber = phoneNumber;

    public void SetCityId(Guid cityId)
    {
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        CityId = cityId;
    }
    public void SetDistrictId(Guid districtId)
    {
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        DistrictId = districtId;
    }
    public void SetTitleId(Guid titleId)
    {
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        TitleId = titleId;
    }

    public void SetDepartmentId(Guid departmentId)
    {
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        DepartmentId = departmentId;
    }
}