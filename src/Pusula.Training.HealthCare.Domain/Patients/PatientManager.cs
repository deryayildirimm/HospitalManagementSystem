using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Patients;

public class PatientManager(IPatientRepository patientRepository) : DomainService
{
    public virtual async Task<Patient> CreateAsync(
        int patientNumber, string firstName, string lastName, DateTime birthDate, EnumGender gender, string identityNumber, string? passportNumber,
        string? nationality = null, string? mobilePhoneNumber = null, EnumPatientTypes? patientType = null, string? mothersName = null, string? fathersName = null,
        string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
        Check.NotNull(identityNumber, nameof(identityNumber));

        var patient = new Patient(
            GuidGenerator.Create(), patientNumber, firstName, lastName, gender, birthDate, identityNumber, passportNumber, nationality, mobilePhoneNumber, patientType,
            mothersName, fathersName, emailAddress, relative, relativePhoneNumber, address, discountGroup
        );

        return await patientRepository.InsertAsync(patient);
    }

    public virtual async Task<Patient> UpdateAsync(
        Guid id, string firstName, string lastName, DateTime birthDate, EnumGender gender, string identityNumber, string? passportNumber, string? nationality = null,
        string? mobilePhoneNumber = null, EnumPatientTypes? patientType = null, string? mothersName = null, string? fathersName = null, string? emailAddress = null,
        EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null, string? concurrencyStamp = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
        Check.NotNull(identityNumber, nameof(identityNumber));

        var patient = await patientRepository.GetAsync(id);

        patient.SetFirstName(firstName);
        patient.SetLastName(lastName);
        patient.SetBirthDate(birthDate);
        patient.SetGender(gender);
        patient.SetIdentityNumber(identityNumber);
        patient.SetPassportNumber(passportNumber);
        patient.SetPatientType(patientType);
        patient.SetMothersName(mothersName);
        patient.SetFathersName(fathersName);
        patient.SetEmailAddress(emailAddress);
        patient.SetNationality(nationality);
        patient.SetMobilePhoneNumber(mobilePhoneNumber);
        patient.SetRelativePhoneNumber(relativePhoneNumber);
        patient.SetRelative(relative);
        patient.SetDiscountGroup(discountGroup);
        patient.SetAddress(address);

        patient.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await patientRepository.UpdateAsync(patient);

    }
}