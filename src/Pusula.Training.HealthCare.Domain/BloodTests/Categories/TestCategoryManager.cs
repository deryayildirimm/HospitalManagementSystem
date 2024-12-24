using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    public class TestCategoryManager(ITestCategoryRepository testCategoryRepository) : DomainService, ITestCategoryManager
    {
        public virtual async Task<TestCategory> CreateAsync(string name, string description, string url, double price)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.CategoryNameMax, BloodTestConst.CategoryNameMin);
            Check.NotNullOrWhiteSpace(description, nameof(description), BloodTestConst.CategoryDescriptionMax, BloodTestConst.CategoryDescriptionMin);
            Check.NotNullOrWhiteSpace(url, nameof(url));
            Check.NotNullOrWhiteSpace(price.ToString(), nameof(price));

            var category = new TestCategory(GuidGenerator.Create(), name, description, url, price);
            return await testCategoryRepository.InsertAsync(category);
        }

        public virtual async Task<TestCategory> UpdateAsync(Guid id, string name, string description, string url, double price)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name), BloodTestConst.CategoryNameMax, BloodTestConst.CategoryNameMin);
            Check.NotNullOrWhiteSpace(description, nameof(description), BloodTestConst.CategoryDescriptionMax, BloodTestConst.CategoryDescriptionMin);
            Check.NotNullOrWhiteSpace(url, nameof(url));
            Check.NotNullOrWhiteSpace(price.ToString(), nameof(price));

            var category = await testCategoryRepository.GetAsync(id);
            category.SetDescription(description);
            category.SetUrl(url);   
            category.SetPrice(price);
            category.SetName(name);

            return await testCategoryRepository.UpdateAsync(category);
        }
    }
}
