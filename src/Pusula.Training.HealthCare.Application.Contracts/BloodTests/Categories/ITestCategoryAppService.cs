using Pusula.Training.HealthCare.BloodTests.Categories;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.BloodTests.Category
{
    public interface ITestCategoryAppService : IApplicationService
    {
        Task<PagedResultDto<TestCategoryDto>> GetListAsync(GetTestCategoriesInput input);
        Task<TestCategoryDto> GetAsync(Guid id);
        Task<TestCategoryDto> CreateAsync(TestCategoryCreateDto input);
        Task<TestCategoryDto> UpdateAsync(TestCategoryUpdateDto input);
    }
}
