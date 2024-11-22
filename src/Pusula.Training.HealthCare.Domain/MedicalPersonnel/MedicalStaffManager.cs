using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public class MedicalStaffManager(IMedicalStaffRepository medicalStaffRepository) : DomainService, IMedicalStaffManager
{

    public virtual async Task<MedicalStaff> CreateAsync(
        Guid cityId, Guid districtId, Guid departmentId, string firstName, string lastName, 
        string identityNumber, DateTime birthDate, EnumGender gender, DateTime startDate, string? email = null, string? phoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), MedicalStaffConsts.FirstNameMaxLength, MedicalStaffConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), MedicalStaffConsts.LastNameMaxLength, MedicalStaffConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), MedicalStaffConsts.IdentityNumberLength, MedicalStaffConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), MedicalStaffConsts.GenderMinValue, MedicalStaffConsts.GenderMaxValue);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        
        var medicalStaff = new MedicalStaff(
            GuidGenerator.Create(), cityId, districtId, departmentId, firstName, lastName, identityNumber, birthDate, gender, startDate, email, phoneNumber
        );

        return await medicalStaffRepository.InsertAsync(medicalStaff);
    }

    public virtual async Task<MedicalStaff> UpdateAsync(Guid id, Guid cityId, Guid districtId, Guid departmentId, 
        string firstName, string lastName, string identityNumber, DateTime birthDate, EnumGender gender, 
        DateTime startDate, string? email = null, string? phoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), MedicalStaffConsts.FirstNameMaxLength, MedicalStaffConsts.FirstNameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), MedicalStaffConsts.LastNameMaxLength, MedicalStaffConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), MedicalStaffConsts.IdentityNumberLength, MedicalStaffConsts.IdentityNumberLength);
        Check.Range((int)gender, nameof(gender), MedicalStaffConsts.GenderMinValue, MedicalStaffConsts.GenderMaxValue);
        Check.NotNullOrWhiteSpace(cityId.ToString(), nameof(cityId));
        Check.NotNullOrWhiteSpace(districtId.ToString(), nameof(districtId));
        Check.NotNullOrWhiteSpace(departmentId.ToString(), nameof(departmentId));
        
        var medicalStaff = await medicalStaffRepository.GetAsync(id);

        medicalStaff.SetDepartmentId(departmentId);
        medicalStaff.SetFirstName(firstName);
        medicalStaff.SetLastName(lastName);
        medicalStaff.SetIdentityNumber(identityNumber);
        medicalStaff.SetBirthDate(birthDate);
        medicalStaff.SetGender(gender);
        medicalStaff.SetStartDate(startDate);
        medicalStaff.SetCityId(cityId);
        medicalStaff.SetDistrictId(districtId);
        medicalStaff.SetEmail(email);
        medicalStaff.SetPhoneNumber(phoneNumber);

        return await medicalStaffRepository.UpdateAsync(medicalStaff);
    }
}