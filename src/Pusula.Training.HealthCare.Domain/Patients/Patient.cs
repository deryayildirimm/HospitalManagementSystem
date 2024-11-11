using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Patients
{
    public class Patient : FullAuditedAggregateRoot<Guid>
    {
        [NotNull] public virtual int PatientNumber { get; set; }

        [NotNull] public virtual string FirstName { get; set; }

        [NotNull] public virtual string LastName { get; set; }

        [CanBeNull] public virtual string? MothersName { get; set; }

        [CanBeNull] public virtual string? FathersName { get; set; }

        [CanBeNull] public virtual string? IdentityNumber { get; set; }

        [NotNull] public virtual string Nationality { get; set; }

        [CanBeNull] public virtual string? PassportNumber { get; set; }

        [NotNull] public virtual DateTime BirthDate { get; set; }

        [CanBeNull] public virtual string? EmailAddress { get; set; }

        [NotNull] public virtual string MobilePhoneNumber { get; set; }
        
        [CanBeNull] public virtual EnumRelative? Relative { get; set; }

        [CanBeNull] public virtual string? RelativePhoneNumber { get; set; }

        [NotNull] public virtual EnumPatientTypes PatientType { get; set; }

        [CanBeNull] public virtual string? Address { get; set; }

        [NotNull] public virtual EnumInsuranceType InsuranceType { get; set; }

        [NotNull] public virtual string InsuranceNo { get; set; }

        [CanBeNull] public virtual EnumDiscountGroup? DiscountGroup { get; set; }

        [NotNull] public virtual EnumGender Gender { get; set; }

        // isAlive

        protected Patient()
        {
            PatientNumber = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Nationality = string.Empty;
            BirthDate = DateTime.Now;
            MobilePhoneNumber = string.Empty;
            PatientType = EnumPatientTypes.NORMAL;
            InsuranceType = EnumInsuranceType.SGK;
            InsuranceNo = string.Empty;
            Gender = EnumGender.FEMALE;
        }

        public Patient(Guid id, int patientNumber, string firstName, string lastName, string nationality, DateTime birthDate, 
            string mobilePhoneNumber, EnumPatientTypes patientType, EnumInsuranceType insuranceType, string insuranceNo, EnumGender gender, 
            string? mothersName = null, string? fathersName = null, string? identityNumber = null, string? passportNumber = null, string? emailAddress = null, EnumRelative? relative = null, string? relativePhoneNumber = null, string? address = null, EnumDiscountGroup? discountGroup = null)
        {
            Check.NotNullOrWhiteSpace(firstName, nameof(firstName), PatientConsts.NameMaxLength, PatientConsts.NameMinLength);
            Check.NotNullOrWhiteSpace(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, PatientConsts.LastNameMinLength);
            Check.NotNullOrWhiteSpace(nationality, nameof(nationality));
            Check.NotNullOrWhiteSpace(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, PatientConsts.MobilePhoneNumberMinLength);
            Check.Range((int)patientType, nameof(patientType), PatientConsts.PatientTypeMinValue, PatientConsts.PatientTypeMaxValue);
            Check.Range((int)insuranceType, nameof(insuranceType), PatientConsts.InsuranceMinValue, PatientConsts.InsuranceMaxValue);
            Check.NotNullOrWhiteSpace(insuranceNo, nameof(insuranceNo), PatientConsts.InsuranceNumberMaxLength, PatientConsts.InsuranceNumberMinLength);

            Id = id;
            PatientNumber = patientNumber;
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
            Relative = relative;
            RelativePhoneNumber = relativePhoneNumber;
            PatientType = patientType;
            Address = address;
            InsuranceType = insuranceType;
            InsuranceNo = insuranceNo;
            DiscountGroup = discountGroup;
            Gender = gender;
        }
    }
}