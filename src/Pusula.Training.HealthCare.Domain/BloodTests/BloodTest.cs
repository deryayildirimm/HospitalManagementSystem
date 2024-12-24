using JetBrains.Annotations;
using Pusula.Training.HealthCare.BloodTests.Reports;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTest : AuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual BloodTestStatus Status { get; private set; }
        [NotNull]
        public virtual DateTime DateCreated { get; private set; }
        [NotNull]
        public virtual Guid DoctorId { get; private set; }
        [NotNull]
        public virtual Guid PatientId { get; private set; }
        [CanBeNull]
        public virtual DateTime DateCompleted { get; private set; }
        public Doctor Doctor { get; private set; } = null!;
        public Patient Patient { get; private set; } = null!;
        public BloodTestReport? BloodTestReport { get; private set; }
        public ICollection<BloodTestCategory> BloodTestCategories { get; private set; } 

        protected BloodTest()
        {
            DoctorId = Guid.Empty;
            PatientId = Guid.Empty;
            Status = BloodTestStatus.Requested;
            DateCreated = DateTime.Now;
            BloodTestCategories = new List<BloodTestCategory>();
        }

        public BloodTest(Guid id, Guid doctorId, Guid patientId, BloodTestStatus status, DateTime dateCreated) : this()
        {
            Id = id;
            SetDoctorId(doctorId);
            SetPatientId(patientId);
            SetStatus(status);
            SetDateCreated(dateCreated);
        }

        public void SetDoctorId(Guid doctorId)
        {
            Check.NotNull(doctorId, nameof(doctorId));
            DoctorId = doctorId;
        }

        public void SetPatientId(Guid patientId)
        {
            Check.NotNull(patientId, nameof(patientId));
            PatientId = patientId;
        }

        public void SetDateCreated(DateTime dateCreated)
        {
            Check.NotNull(dateCreated, nameof(dateCreated));
            DateCreated = dateCreated;
        }

        public void SetDateCompleted(DateTime? dateCompleted)
        {
            DateCompleted = (DateTime)dateCompleted!;
        }

        public void SetStatus(BloodTestStatus status)
        {
            Check.Range((int)status, nameof(status), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);
            Status = status;
        }

        public void AddCategory(Guid testCategoryId)
        {
            Check.NotNull(testCategoryId, nameof(testCategoryId));

            if (IsInCategory(testCategoryId))
            {
                return;
            }

            BloodTestCategories.Add(new BloodTestCategory(bloodTestId: Id, testCategoryId));
        }

        public void RemoveCategory(Guid testCategoryId)
        {
            Check.NotNull(testCategoryId, nameof(testCategoryId));

            if (!IsInCategory(testCategoryId))
            {
                return;
            }

            BloodTestCategories.RemoveAll(x => x.TestCategoryId == testCategoryId);
        }

        public void RemoveAllCategoriesExceptGivenIds(List<Guid> testCategoryIds)
        {
            Check.NotNullOrEmpty(testCategoryIds, nameof(testCategoryIds));

            BloodTestCategories.RemoveAll(x => !testCategoryIds.Contains(x.TestCategoryId));
        }

        public void RemoveAllCategories()
        {
            BloodTestCategories.RemoveAll(x => x.BloodTestId == Id);
        }

        private bool IsInCategory(Guid testCategoryId)
        {
            return BloodTestCategories.Any(x => x.TestCategoryId == testCategoryId);
        }
    }
}
