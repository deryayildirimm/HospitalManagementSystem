using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Patients;

public class PatientManager(IPatientRepository patientRepository) : DomainService
{
    public virtual async Task<Patient> CreateAsync(
        string firstName, string lastName, DateTime birthDate, string identityNumber, string emailAddress, string mobilePhoneNumber, int gender, string? homePhoneNumber = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), PatientConsts.FirstNameMaxLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), PatientConsts.LastNameMaxLength);
        Check.NotNull(birthDate, nameof(birthDate));
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber));
        Check.Length(identityNumber, nameof(identityNumber), PatientConsts.IdentityNumberMaxLength);
        Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), PatientConsts.EmailAddressMaxLength);
        Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber));
        Check.Length(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength);
        Check.Range(gender, nameof(gender), PatientConsts.GenderMinLength, PatientConsts.GenderMaxLength);

        var patient = new Patient(
         GuidGenerator.Create(),
         firstName, lastName, birthDate, identityNumber, emailAddress, mobilePhoneNumber, gender, homePhoneNumber
         );

        return await patientRepository.InsertAsync(patient);
    }

    public virtual async Task<Patient> UpdateAsync(
        Guid id,
        string firstName, string lastName, DateTime birthDate, string identityNumber, string emailAddress, string mobilePhoneNumber, int gender, string? homePhoneNumber = null, [CanBeNull] string? concurrencyStamp = null
    )
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), PatientConsts.FirstNameMaxLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), PatientConsts.LastNameMaxLength);
        Check.NotNull(birthDate, nameof(birthDate));
        Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber));
        Check.Length(identityNumber, nameof(identityNumber), PatientConsts.IdentityNumberMaxLength);
        Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), PatientConsts.EmailAddressMaxLength);
        Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber));
        Check.Length(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength);
        Check.Range(gender, nameof(gender), PatientConsts.GenderMinLength, PatientConsts.GenderMaxLength);

        var patient = await patientRepository.GetAsync(id);

        patient.FirstName = firstName;
        patient.LastName = lastName;
        patient.BirthDate = birthDate;
        patient.IdentityNumber = identityNumber;
        patient.EmailAddress = emailAddress;
        patient.MobilePhoneNumber = mobilePhoneNumber;
        patient.Gender = gender;
        patient.HomePhoneNumber = homePhoneNumber;

        patient.SetConcurrencyStampIfNotNull(concurrencyStamp);
        return await patientRepository.UpdateAsync(patient);
    }

}