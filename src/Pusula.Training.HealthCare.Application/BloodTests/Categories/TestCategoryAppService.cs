using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.BloodTests.Category;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.BloodTests.Categories
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.TestCategories.Default)]
    public class TestCategoryAppService(ITestCategoryRepository testCategoryRepository,
        ITestCategoryManager testCategoryManager) : HealthCareAppService, ITestCategoryAppService
    {
        public virtual async Task<PagedResultDto<TestCategoryDto>> GetListAsync(GetTestCategoriesInput input)
        {
            var totalCount = await testCategoryRepository.GetCountAsync(input.FilterText, input.Name, input.Description, input.Url, input.Price);
            var items = await testCategoryRepository.GetListAsync(input.FilterText, input.Name, input.Description, input.Url, input.Price);

            return new PagedResultDto<TestCategoryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<TestCategory>, List<TestCategoryDto>>(items)
            };
        }

        public virtual async Task<TestCategoryDto> GetAsync(Guid id) => ObjectMapper.Map<TestCategory, TestCategoryDto>(await testCategoryRepository.GetAsync(id));
        
    }
}
