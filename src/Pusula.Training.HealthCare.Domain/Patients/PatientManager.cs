using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Patients;

public class PatientManager(IPatientRepository patientRepository ) : DomainService
{
    public virtual async Task<Patient> CreateAsync(int patientNumber, string firstName, string lastName, string nationality, DateTime birthDate, string mobilePhoneNumber, EnumPatientTypes patientType, EnumInsuranceType insuranceType, string insuranceNo, EnumGender gender, 
        string? mothersName = null, string? fathersName = null, string? identityNumber = null, string? passportNumber = null, string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null)
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(nationality, nameof(nationality));
        Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, PatientConsts.MobilePhoneNumberMinLength);
        Check.Range((int)patientType, nameof(patientType), PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue);
        Check.Range((int)insuranceType, nameof(insuranceType), PatientConsts.InsuranceMinValue, PatientConsts.InsuranceMaxValue);
        Check.NotNullOrWhiteSpace(insuranceNo, nameof(insuranceNo), PatientConsts.InsuranceNumberMaxLength, PatientConsts.InsuranceNumberMinLength);


        var patient = new Patient(
            GuidGenerator.Create(), patientNumber,
            firstName, lastName, nationality, birthDate, mobilePhoneNumber, patientType, insuranceType, insuranceNo, gender, mothersName, fathersName, identityNumber, passportNumber, emailAddress, relative, relativePhoneNumber, address, discountGroup
        );

        return await patientRepository.InsertAsync(patient);
    }

    public virtual async Task<Patient> UpdateAsync(
        Guid id,
        string firstName, string lastName, string nationality, DateTime birthDate, 
        string mobilePhoneNumber, EnumPatientTypes patientType, EnumInsuranceType insuranceType, string insuranceNo, EnumGender gender, 
        bool isDeleted,
        string? mothersName = null, string? fathersName = null, string? identityNumber = null, string? passportNumber = null, 
        string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, 
        EnumDiscountGroup? discountGroup = null,  [CanBeNull] string? concurrencyStamp = null
    )
    {
        Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
        Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
        Check.NotNullOrWhiteSpace(nationality, nameof(nationality));
        Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, PatientConsts.MobilePhoneNumberMinLength);
        Check.Range((int)patientType, nameof(patientType), PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue);
        Check.Range((int)insuranceType, nameof(insuranceType), PatientConsts.InsuranceMinValue, PatientConsts.InsuranceMaxValue);
        Check.NotNullOrWhiteSpace(insuranceNo, nameof(insuranceNo), PatientConsts.InsuranceNumberMaxLength, PatientConsts.InsuranceNumberMinLength);
        Check.NotNull(isDeleted, nameof(isDeleted));
       
            var patient = await patientRepository.GetAsync(id);

            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.MothersName = mothersName;
            patient.FathersName = fathersName;
            patient.IdentityNumber = identityNumber;
            patient.Nationality = nationality;
            patient.PassportNumber = passportNumber;
            patient.BirthDate = birthDate;
            patient.EmailAddress = emailAddress;
            patient.MobilePhoneNumber = mobilePhoneNumber;
            patient.Relative = relative;
            patient.RelativePhoneNumber = relativePhoneNumber;
            patient.PatientType = patientType;
            patient.Address = address;
            patient.InsuranceType = insuranceType;
            patient.InsuranceNo = insuranceNo;
            patient.DiscountGroup = discountGroup;
            patient.Gender = gender;
            patient.IsDeleted = isDeleted; 
            
            patient.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await patientRepository.UpdateAsync(patient);
        
        
    }
}