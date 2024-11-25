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

    }
}
