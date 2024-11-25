using JetBrains.Annotations;
using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTest : AuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual BloodTestStatus Status { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public virtual DateTime DateCompleted { get; set; }
        public virtual Guid DoctorId { get; set; }
        public virtual Guid PatientId { get; set; }
        public virtual Guid TestCategoryId { get; set; }

        protected BloodTest()
        {
            DoctorId = Guid.Empty;
            PatientId = Guid.Empty;
            TestCategoryId = Guid.Empty;
            Status = BloodTestStatus.Requested;
            DateCreated = DateTime.Now;
        }

        public BloodTest(Guid id, Guid doctorId, Guid patientId, Guid testCategoryId, BloodTestStatus status, DateTime dateCreated)
        {
            Check.NotNullOrWhiteSpace(doctorId.ToString(), nameof(doctorId));
            Check.NotNullOrWhiteSpace(patientId.ToString(), nameof(patientId));
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(dateCreated.ToString(), nameof(dateCreated));
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            TestCategoryId = testCategoryId;
            Status = status;
            DateCreated = dateCreated;
        }
    }
}
