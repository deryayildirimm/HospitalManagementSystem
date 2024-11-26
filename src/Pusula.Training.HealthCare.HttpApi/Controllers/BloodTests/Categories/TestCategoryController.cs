using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Category;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Categories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TestCategory")]
    [Route("api/app/bloodtest/testcategory")]
    public class TestCategoryController(ITestCategoryAppService testCategoryAppService) : HealthCareController, ITestCategoryAppService
    {
        [HttpGet]
        [Route("{id}")]
        public Task<TestCategoryDto> GetAsync(Guid id) => testCategoryAppService.GetAsync(id);

        [HttpGet]
        public async Task<PagedResultDto<TestCategoryDto>> GetListAsync(GetTestCategoriesInput input) => await testCategoryAppService.GetListAsync(input);
    }
}
