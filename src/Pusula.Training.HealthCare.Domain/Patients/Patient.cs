using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Patients
{
    public class Patient : FullAuditedAggregateRoot<Guid>
    {
        [NotNull] public virtual int PatientNumber { get; private set; }

        [NotNull] public virtual string FirstName { get; private set; } = string.Empty;

        [NotNull] public virtual string LastName { get; private set; } = string.Empty;

        [CanBeNull] public virtual string? MothersName { get; private set; }

        [CanBeNull] public virtual string? FathersName { get; private set; }

        [NotNull] public virtual string IdentityNumber { get; private set; } = string.Empty;

        [CanBeNull] public virtual string? PassportNumber { get; private set; } 

        [CanBeNull] public virtual string? Nationality { get; private set; }

        [NotNull] public virtual DateTime BirthDate { get; private set; }

        [CanBeNull] public virtual string? EmailAddress { get; private set; }

        [CanBeNull] public virtual string? MobilePhoneNumber { get; private set; }
        
        [CanBeNull] public virtual EnumRelative? Relative { get; private set; }

        [CanBeNull] public virtual string? RelativePhoneNumber { get; private set; }

        [CanBeNull] public virtual EnumPatientTypes? PatientType { get; private set; }

        [CanBeNull] public virtual string? Address { get; private set; }

        [CanBeNull] public virtual EnumDiscountGroup? DiscountGroup { get; private set; }

        [NotNull] public virtual EnumGender Gender { get; private set; }

        protected Patient()
        {
            PatientNumber = 0;
            BirthDate = DateTime.Now;
            Gender = EnumGender.FEMALE;
        }

        public Patient(Guid id, int patientNumber, string firstName, string lastName, EnumGender gender, DateTime birthDate, string identityNumber, string? passportNumber,
            string? nationality = null, string? mobilePhoneNumber = null, EnumPatientTypes? patientType = null, string? mothersName = null, string? fathersName = null, 
            string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null)
        {
            SetId(id);
            SetPatientNumber(patientNumber);
            SetFirstName(firstName);
            SetLastName(lastName);
            SetMothersName(mothersName);
            SetFathersName(fathersName);
            SetIdentityNumber(identityNumber);
            SetPassportNumber(passportNumber);
            SetNationality(nationality);
            SetBirthDate(birthDate);
            SetEmailAddress(emailAddress);
            SetMobilePhoneNumber(mobilePhoneNumber);
            SetRelative(relative);
            SetRelativePhoneNumber(relativePhoneNumber);
            SetPatientType(patientType);
            SetAddress(address);
            SetDiscountGroup(discountGroup);
            SetGender(gender);
        }

        public void SetId(Guid id)
        {
            Id = id;
        }

        public void SetPatientNumber(int patientNumber)
        {
            PatientNumber = patientNumber;
        }

        public void SetFirstName(string firstName)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
            LastName = lastName;
        }
        public void SetBirthDate(DateTime birthDate)
        {
            BirthDate = birthDate;
        }

        public void SetGender(EnumGender gender)
        {
            Gender = gender;
        }
        public void SetIdentityNumber(string identityNumber)
        {
            Check.NotNullOrWhiteSpace(identityNumber, nameof(identityNumber));
            IdentityNumber = identityNumber;
        }

        public void SetPassportNumber(string? passportNumber)
        {
            PassportNumber = passportNumber;
        }

        public void SetMothersName(string? mothersName)
        {
            MothersName = mothersName;
        }

        public void SetFathersName(string? fathersName)
        {
            FathersName = fathersName;
        }

        public void SetNationality(string? nationality)
        {
            Nationality = nationality;
        }

        public void SetEmailAddress(string? emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void SetMobilePhoneNumber(string? mobilePhoneNumber)
        {
            MobilePhoneNumber = mobilePhoneNumber;
        }

        public void SetRelative(EnumRelative? relative)
        {
            Relative = relative;
        }

        public void SetRelativePhoneNumber(string? relativePhoneNumber)
        {
            RelativePhoneNumber = relativePhoneNumber;
        }

        public void SetPatientType(EnumPatientTypes? patientType)
        {
            PatientType = patientType;
        }

        public void SetAddress(string? address)
        {
            Address = address;
        }

        public void SetDiscountGroup(EnumDiscountGroup? discountGroup)
        {
            DiscountGroup = discountGroup;
        }
    }
}