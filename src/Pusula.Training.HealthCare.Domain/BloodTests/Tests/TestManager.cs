using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestManager(ITestRepository testRepository) : DomainService, ITestManager
    {
        public virtual async Task<Test> CreateAsync(Guid testCategoryId, string name, double minValue, double maxValue)
        {
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.TestNameMax, BloodTestConst.TestNameMin);
            Check.NotNullOrWhiteSpace(minValue.ToString(), nameof(minValue));
            Check.NotNullOrWhiteSpace(maxValue.ToString(), nameof(maxValue));

            var test = new Test(GuidGenerator.Create(), testCategoryId, name, minValue, maxValue);
            return await testRepository.InsertAsync(test);
        }

        public virtual async Task<Test> UpdateAsync(Guid id, Guid testCategoryId, string name, double minValue, double maxValue)
        {
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.TestNameMax, BloodTestConst.TestNameMin);
            Check.NotNullOrWhiteSpace(minValue.ToString(), nameof(minValue));
            Check.NotNullOrWhiteSpace(maxValue.ToString(), nameof(maxValue));

            var test = await testRepository.GetAsync(id);
            test.SetName(name);
            test.SetMinValue(minValue);
            test.SetMaxValue(maxValue);
            test.SetTestCategoryId(testCategoryId);

            return await testRepository.UpdateAsync(test);
        }
    }
}
