using System;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaff : FullAuditedAggregateRoot<Guid>
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
    [NotNull]
    public virtual Guid DistrictId { get; protected set; }
    [NotNull]
    public virtual Guid DepartmentId { get; protected set; }

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
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), MedicalStaffConsts.FirstNameMaxLength, MedicalStaffConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), MedicalStaffConsts.LastNameMaxLength, MedicalStaffConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), MedicalStaffConsts.IdentityNumberLength, MedicalStaffConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), MedicalStaffConsts.GenderMinValue, MedicalStaffConsts.GenderMaxValue);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        
        
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        IdentityNumber = identityNumber;
        BirthDate = birthDate;
        Gender = gender;
        StartDate = startDate;
        CityId = cityId;
        DistrictId = districtId;
        Email = email;
        PhoneNumber = phoneNumber;
        DepartmentId = departmentId;
    }

    public void SetFirstName(string firstName) => FirstName = firstName;
    public void SetLastName(string lastName) => LastName = lastName;
    public void SetIdentityNumber(string identityNumber) => IdentityNumber = identityNumber;
    public void SetBirthDate(DateTime birthDate) => BirthDate = birthDate;
    public void SetGender(EnumGender gender) => Gender = gender;
    public void SetStartDate(DateTime startDate) => StartDate = startDate;
    public void SetEmail(string? email) => Email = email;
    public void SetPhoneNumber(string? phoneNumber) => PhoneNumber = phoneNumber;
    public void SetCityId(Guid cityId) => CityId = cityId;
    public void SetDistrictId(Guid districtId) => DistrictId = districtId;
    public void SetDepartmentId(Guid departmentId) => DepartmentId = departmentId;
}