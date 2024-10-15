using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Patients
{
    public class Patient : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string FirstName { get; set; }

        [NotNull]
        public virtual string LastName { get; set; }

        public virtual DateTime BirthDate { get; set; }

        [NotNull]
        public virtual string IdentityNumber { get; set; }

        [NotNull]
        public virtual string EmailAddress { get; set; }

        [NotNull]
        public virtual string MobilePhoneNumber { get; set; }

        [CanBeNull]
        public virtual string? HomePhoneNumber { get; set; }

        public virtual int Gender { get; set; }

        protected Patient()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            IdentityNumber = string.Empty;
            EmailAddress = string.Empty;
            MobilePhoneNumber = string.Empty;
        }

        public Patient(Guid id, string firstName, string lastName, DateTime birthDate, string identityNumber, string emailAddress, string mobilePhoneNumber, int gender, string? homePhoneNumber = null)
        {

            Id = id;
            Check.NotNull(firstName, nameof(firstName));
            Check.Length(firstName, nameof(firstName), PatientConsts.FirstNameMaxLength, 0);
            Check.NotNull(lastName, nameof(lastName));
            Check.Length(lastName, nameof(lastName), PatientConsts.LastNameMaxLength, 0);
            Check.NotNull(identityNumber, nameof(identityNumber));
            Check.Length(identityNumber, nameof(identityNumber), PatientConsts.IdentityNumberMaxLength, 0);
            Check.NotNull(emailAddress, nameof(emailAddress));
            Check.Length(emailAddress, nameof(emailAddress), PatientConsts.EmailAddressMaxLength, 0);
            Check.NotNull(mobilePhoneNumber, nameof(mobilePhoneNumber));
            Check.Length(mobilePhoneNumber, nameof(mobilePhoneNumber), PatientConsts.MobilePhoneNumberMaxLength, 0);
            if (gender < PatientConsts.GenderMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(gender), gender, "The value of 'gender' cannot be lower than " + PatientConsts.GenderMinLength);
            }

            if (gender > PatientConsts.GenderMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(gender), gender, "The value of 'gender' cannot be greater than " + PatientConsts.GenderMaxLength);
            }

            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            IdentityNumber = identityNumber;
            EmailAddress = emailAddress;
            MobilePhoneNumber = mobilePhoneNumber;
            Gender = gender;
            HomePhoneNumber = homePhoneNumber;
        }
    }
}