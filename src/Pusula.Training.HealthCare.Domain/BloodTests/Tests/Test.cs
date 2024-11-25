using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class Test : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual double MinValue { get; set; } 
        [NotNull]
        public virtual double MaxValue { get; set; } 

        public virtual Guid TestCategoryId { get; set; } 

        protected Test()
        {
            TestCategoryId = Guid.Empty;
            Name = string.Empty;
            MinValue = double.MinValue;
            MaxValue = double.MaxValue;
        }

        public Test(Guid id, Guid testCategoryId, string name, double minValue, double maxValue)
        {
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.TestNameMax, BloodTestConst.TestNameMin);
            Check.NotNullOrWhiteSpace(minValue.ToString(), nameof(minValue));
            Check.NotNullOrWhiteSpace(maxValue.ToString(), nameof(maxValue));

            Id = id;
            TestCategoryId = testCategoryId; 
            Name = name; 
            MinValue = minValue; 
            MaxValue = maxValue;
        }

    }
}
