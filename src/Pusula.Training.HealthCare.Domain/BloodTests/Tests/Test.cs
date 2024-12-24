using Pusula.Training.HealthCare.BloodTests.Categories;
using System;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class Test : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Name { get; private set; } = string.Empty;    
        [NotNull]
        public virtual double MinValue { get; private set; } 
        [NotNull]
        public virtual double MaxValue { get; private set; } 
        [NotNull]
        public virtual Guid TestCategoryId { get; private set; }
        public TestCategory TestCategory { get; private set; } = null!;

        protected Test()
        {
            TestCategoryId = Guid.Empty;
            MinValue = double.MinValue;
            MaxValue = double.MaxValue;
        }

        public Test(Guid id, Guid testCategoryId, string name, double minValue, double maxValue)
        {
            Id = id;
            SetTestCategoryId(testCategoryId); 
            SetName(name); 
            SetMinValue(minValue); 
            SetMaxValue(maxValue);
        }

        public void SetName(string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.TestNameMax, BloodTestConst.TestNameMin);
            Name = name;
        }

        public void SetMinValue(double minValue)
        {
            Check.NotNullOrWhiteSpace(minValue.ToString(), nameof(minValue));
            MinValue = minValue;
        }

        public void SetMaxValue(double maxValue)
        {
            Check.NotNullOrWhiteSpace(maxValue.ToString(), nameof(maxValue));
            MaxValue = maxValue;
        }

        public void SetTestCategoryId(Guid testCategoryId)
        {
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            TestCategoryId = testCategoryId;
        }
    }
}
