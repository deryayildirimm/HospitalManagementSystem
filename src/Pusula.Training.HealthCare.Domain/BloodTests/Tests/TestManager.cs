using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Tests
{
    public class TestManager(ITestRepository testRepository) : DomainService, ITestManager
    {
        public async Task<Test> CreateAsync(Guid testCategoryId, string name, double minValue, double maxValue)
        {
            Check.NotNullOrWhiteSpace(testCategoryId.ToString(), nameof(testCategoryId));
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.TestNameMax, BloodTestConst.TestNameMin);
            Check.NotNullOrWhiteSpace(minValue.ToString(), nameof(minValue));
            Check.NotNullOrWhiteSpace(maxValue.ToString(), nameof(maxValue));

            var test = new Test(GuidGenerator.Create(), testCategoryId, name, minValue, maxValue);
            return await testRepository.InsertAsync(test);
        }
    }
}
