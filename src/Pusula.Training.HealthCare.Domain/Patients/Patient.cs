using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Patients
{
    public class Patient : FullAuditedAggregateRoot<Guid>
    {
        public virtual string? PatientNumber { get; set; } // is this id of patient or should we generate new number for it?

        [NotNull] public virtual string FirstName { get; set; }

        [NotNull] public virtual string LastName { get; set; }

        [CanBeNull] public virtual string? MothersName { get; set; }

        [CanBeNull] public virtual string? FathersName { get; set; }

        [NotNull] public virtual string IdentityNumber { get; set; }

        [NotNull] public virtual EnumNationality Nationality { get; set; }

        [NotNull] public virtual string PassportNumber { get; set; }

        [NotNull] public virtual DateTime BirthDate { get; set; }

        [CanBeNull] public virtual string? EmailAddress { get; set; }

        [NotNull] public virtual string MobilePhoneNumber { get; set; }

        [CanBeNull] public virtual string? HomePhoneNumber { get; set; }

        [NotNull] public virtual EnumPatientTypes PatientType { get; set; }

        [CanBeNull] public virtual string? Address { get; set; }

        [NotNull] public virtual EnumInsuranceType InsuranceType { get; set; }

        [NotNull] public virtual string InsuranceNo { get; set; }

        [CanBeNull] public virtual EnumDiscountGroup? DiscountGroup { get; set; }

        [NotNull] public virtual EnumGender Gender { get; set; }


        protected Patient()
        {
            PatientNumber = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            IdentityNumber = string.Empty;
            Nationality = EnumNationality.TURKISH;
            PassportNumber = string.Empty;
            BirthDate = DateTime.Now;
            MobilePhoneNumber = string.Empty;
            PatientType = EnumPatientTypes.NORMAL;
            InsuranceType = EnumInsuranceType.SGK;
            InsuranceNo = string.Empty;
            Gender = EnumGender.FEMALE;
        }
        
        public Patient(Guid id, string firstName, string lastName, DateTime birthDate, string identityNumber,
            string? emailAddress, string mobilePhoneNumber, EnumGender gender, string? homePhoneNumber = null)
        {
            Id = id;
            Check.NotNull(firstName, nameof(firstName));
            Check.Length(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);

            Check.NotNull(lastName, nameof(lastName));
            Check.Length(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, 0);

            Check.NotNull(identityNumber, nameof(identityNumber));
            Check.Length(identityNumber, nameof(identityNumber), PatientConsts.IdentityNumberLength, 0);

            Check.NotNull(emailAddress, nameof(emailAddress));
            Check.Length(emailAddress, nameof(emailAddress), PatientConsts.EmailAddressMaxLength, 0);

            Check.NotNull(mobilePhoneNumber, nameof(mobilePhoneNumber));
            Check.Length(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, 0);
            
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            IdentityNumber = identityNumber;
            EmailAddress = emailAddress;
            MobilePhoneNumber = mobilePhoneNumber;
            Gender = gender;
            HomePhoneNumber = homePhoneNumber;
        }

        public Patient(Guid id, string firstName, string lastName, string identityNumber, EnumNationality nationality, string passportNumber, DateTime birthDate, 
            string mobilePhoneNumber, EnumPatientTypes patientType, EnumInsuranceType insuranceType, string insuranceNo, EnumGender gender, 
            string? mothersName = null, string? fathersName = null, string? emailAddress = null, string? homePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);

            Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber), PatientConsts.IdentityNumberLength,
                PatientConsts.IdentityNumberLength);
            Check.Range((int)nationality, nameof(nationality), PatientConsts.NationalityMinLength, PatientConsts.NationalityMaxLength);
            Check.NotNullOrWhiteSpace(passportNumber, nameof(passportNumber), PatientConsts.PassportNumberMaxLength, PatientConsts.PassportNumberMinLength);
            Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, PatientConsts.MobilePhoneNumberMinLength);
            Check.Range((int)patientType, nameof(patientType), PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue);
            Check.Range((int)insuranceType, nameof(insuranceType), PatientConsts.InsuranceMinValue, PatientConsts.InsuranceMaxValue);
            Check.NotNullOrWhiteSpace(insuranceNo, nameof(insuranceNo), PatientConsts.InsuranceNumberMaxLength, PatientConsts.InsuranceNumberMinLength);
            
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            MothersName = mothersName;
            FathersName = fathersName;
            IdentityNumber = identityNumber;
            Nationality = nationality;
            PassportNumber = passportNumber;
            BirthDate = birthDate;
            EmailAddress = emailAddress;
            MobilePhoneNumber = mobilePhoneNumber;
            HomePhoneNumber = homePhoneNumber;
            PatientType = patientType;
            Address = address;
            InsuranceType = insuranceType;
            InsuranceNo = insuranceNo;
            DiscountGroup = discountGroup;
            Gender = gender;
        }
    }
}