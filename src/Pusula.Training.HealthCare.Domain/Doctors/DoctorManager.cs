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
        Guid titleId, Guid departmentId, string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender, 
        int yearOfExperience, string city, string district, string? email = null, string? phoneNumber = null)
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
        
        var doctor = new Doctor(
            GuidGenerator.Create(), titleId, departmentId, firstName, lastName, identityNumber, birthDate, gender, yearOfExperience, city, district, email, phoneNumber
        );

        return await doctorRepository.InsertAsync(doctor);
    }

    public virtual async Task<Doctor> UpdateAsync(Guid id, Guid titleId, Guid departmentId, string firstName,
        string lastName, string identityNumber, DateTime birthDate, EnumGender gender, int yearOfExperience, string city,
        string district, string? email = null, string? phoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), DoctorConsts.FirstNameMaxLength, DoctorConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), DoctorConsts.LastNameMaxLength, DoctorConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), DoctorConsts.IdentityNumberLength, DoctorConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), DoctorConsts.GenderMinValue, DoctorConsts.GenderMaxValue);
        Check.Range(yearOfExperience, nameof(yearOfExperience), 0, 100);
        Check.NotNullOrWhiteSpace(city, nameof(city), DoctorConsts.CityMaxLength, DoctorConsts.CityMinLength);
        Check.NotNullOrWhiteSpace(district, nameof(district), DoctorConsts.DistrictMaxLength, DoctorConsts.DistrictMinLength);
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(titleId.ToString(), nameof(titleId));
        
        var doctor = await doctorRepository.GetAsync(id);

        doctor.TitleId = titleId;
        doctor.DepartmentId = departmentId;
        doctor.FirstName = firstName;
        doctor.LastName = lastName;
        doctor.IdentityNumber = identityNumber;
        doctor.BirthDate = birthDate;
        doctor.Gender = gender;
        doctor.YearOfExperience = yearOfExperience;
        doctor.City = city;
        doctor.District = district;
        doctor.Email = email;
        doctor.PhoneNumber = phoneNumber;

        return await doctorRepository.UpdateAsync(doctor);
    }
    
}