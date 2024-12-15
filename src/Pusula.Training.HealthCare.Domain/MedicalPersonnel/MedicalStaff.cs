using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaff : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public virtual string FirstName { get; private set; } = null!;
    [NotNull]
    public virtual string LastName { get; private set; } = null!;
    [NotNull]
    public virtual string IdentityNumber { get; private set; } = null!;
    [NotNull]
    public virtual DateTime BirthDate { get; private set; }
    [NotNull]
    public virtual EnumGender Gender { get; private set; }
    [CanBeNull]
    public virtual string? Email { get; private set; }
    [CanBeNull]
    public virtual string? PhoneNumber { get; private set; }
    [NotNull]
    public virtual DateTime StartDate { get; private set; }
    [NotNull]
    public virtual Guid CityId { get; private set; }
    public virtual City City { get; private set; } = null!;
    [NotNull]
    public virtual Guid DistrictId { get; private set; }
    public virtual District District { get; private set; } = null!;
    [NotNull]
    public virtual Guid DepartmentId { get; private set; }
    public virtual Department Department { get; private set; } = null!;

    protected MedicalStaff()
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
    }

    public MedicalStaff(Guid id, Guid cityId, Guid districtId, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender,
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
        SetDepartmentId(departmentId);
    }

    public void SetFirstName(string firstName)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), MedicalStaffConsts.FirstNameMaxLength,
            MedicalStaffConsts.FirstNameMinLength);
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), MedicalStaffConsts.LastNameMaxLength,
            MedicalStaffConsts.LastNameMinLength);
        LastName = lastName;    
    }

    public void SetIdentityNumber(string identityNumber)
    {
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), MedicalStaffConsts.IdentityNumberLength,
            MedicalStaffConsts.IdentityNumberLength);
        IdentityNumber = identityNumber;
    }

    public void SetBirthDate(DateTime birthDate) => BirthDate = birthDate;

    public void SetGender(EnumGender gender)
    {
        Check.Range((int)gender, nameof(gender), MedicalStaffConsts.GenderMinValue, MedicalStaffConsts.GenderMaxValue);
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
    public void SetDepartmentId(Guid departmentId)
    {
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        DepartmentId = departmentId;
    }
}