using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Doctors;

public class DoctorManager(IDoctorRepository doctorRepository) : DomainService
{

    public virtual async Task<Doctor> CreateAsync(
        Guid cityId, Guid districtId, Guid titleId, Guid departmentId, string firstName, string lastName, 
        string identityNumber, DateTime birthDate, EnumGender gender, DateTime startDate, string? email = null, string? phoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), DoctorConsts.FirstNameMaxLength, DoctorConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), DoctorConsts.LastNameMaxLength, DoctorConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), DoctorConsts.IdentityNumberLength, DoctorConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), DoctorConsts.GenderMinValue, DoctorConsts.GenderMaxValue);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        
        var doctor = new Doctor(
            GuidGenerator.Create(), cityId, districtId, titleId, departmentId, firstName, lastName, identityNumber, birthDate, gender, startDate, email, phoneNumber
        );

        return await doctorRepository.InsertAsync(doctor);
    }

    public virtual async Task<Doctor> UpdateAsync(Guid id, Guid cityId, Guid districtId, Guid titleId, Guid departmentId, 
        string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender, 
        DateTime startDate, string? email = null, string? phoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), DoctorConsts.FirstNameMaxLength, DoctorConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), DoctorConsts.LastNameMaxLength, DoctorConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), DoctorConsts.IdentityNumberLength, DoctorConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), DoctorConsts.GenderMinValue, DoctorConsts.GenderMaxValue);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        
        var doctor = await doctorRepository.GetAsync(id);

        doctor.SetTitleId(titleId);
        doctor.SetDepartmentId(departmentId);
        doctor.SetFirstName(firstName);
        doctor.SetLastName(lastName);
        doctor.SetIdentityNumber(identityNumber);
        doctor.SetBirthDate(birthDate);
        doctor.SetGender(gender);
        doctor.SetStartDate(startDate);
        doctor.SetCityId(cityId);
        doctor.SetDistrictId(districtId);
        doctor.SetEmail(email);
        doctor.SetPhoneNumber(phoneNumber);

        return await doctorRepository.UpdateAsync(doctor);
    }
    
}