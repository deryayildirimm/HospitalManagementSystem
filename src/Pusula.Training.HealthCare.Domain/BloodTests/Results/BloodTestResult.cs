using System;
using System.Diagnostics.CodeAnalysis;
using Pusula.Training.HealthCare.BloodTests.Tests;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Results
{
    public class BloodTestResult : AuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual double Value { get; private set; }
        [NotNull]
        public virtual BloodResultStatus BloodResultStatus { get; private set; }
        [NotNull]
        public virtual Guid TestId { get; private set; }
        public Test Test { get; private set; } = null!;

        protected BloodTestResult()
        {
            TestId = Guid.Empty;
            Value = 0;
            BloodResultStatus = BloodResultStatus.None;
        }

        public BloodTestResult(Guid id, Guid testId, double value, BloodResultStatus bloodResultStatus)
        {
            Id = id;
            SetTestId(testId);
            SetValue(value);
            SetBloodResultStatus(bloodResultStatus);
        }

        public void SetTestId(Guid testId)
        {
            Check.NotNullOrWhiteSpace(testId.ToString(), nameof(testId));
            TestId = testId;
        }

        public void SetValue(double value)
        {
            Check.NotNullOrWhiteSpace(value.ToString(), nameof(value));
            Value = value;
        }

        public void SetBloodResultStatus(BloodResultStatus bloodResultStatus)
        {
            Check.Range((int)bloodResultStatus, nameof(bloodResultStatus), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);
            BloodResultStatus = bloodResultStatus;
        }
    }
}
