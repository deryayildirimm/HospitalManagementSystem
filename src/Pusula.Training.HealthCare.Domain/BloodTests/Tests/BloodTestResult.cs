using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class BloodTestResult : AuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual double Value { get; set; }
        [NotNull]
        public virtual BloodResultStatus BloodResultStatus { get; set; }
        public virtual Guid BloodTestId { get; set; } 
        public virtual Guid TestId { get; set; } 

        protected BloodTestResult()
        {
            BloodTestId = Guid.Empty;
            TestId = Guid.Empty;
            Value = 0;
            BloodResultStatus = BloodResultStatus.Normal;
        }

        public BloodTestResult(Guid id, Guid bloodTestId, Guid testId, double value, BloodResultStatus bloodResultStatus)
        {
            Check.NotNullOrWhiteSpace(bloodTestId.ToString(), nameof(bloodTestId));
            Check.NotNullOrWhiteSpace(testId.ToString(), nameof(testId));
            Check.NotNullOrWhiteSpace(value.ToString(), nameof(value));
            Check.Range((int)bloodResultStatus, nameof(bloodResultStatus), BloodTestConst.BloodTestStatusMin, BloodTestConst.BloodTestStatusMax);

            Id = id;
            BloodTestId = bloodTestId;
            TestId = testId;
            Value = value;
            BloodResultStatus = bloodResultStatus;
        }
    }
}
