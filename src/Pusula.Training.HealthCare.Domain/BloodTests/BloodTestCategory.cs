using Pusula.Training.HealthCare.BloodTests.Categories;
using System;
using System.Net;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.BloodTests
{
    public class BloodTestCategory : Entity
    {
        public Guid BloodTestId { get; set; }
        public BloodTest BloodTest { get; set; } = null!;

        public Guid TestCategoryId { get; set; }
        public TestCategory TestCategory { get; set; } = null!;

        private BloodTestCategory(){}

        public BloodTestCategory(Guid bloodTestId, Guid testCategoryId)
        {
            BloodTestId = bloodTestId;
            TestCategoryId = testCategoryId;
        }

        public override object?[] GetKeys() => new object[] { BloodTestId, TestCategoryId };
        
    }
}
