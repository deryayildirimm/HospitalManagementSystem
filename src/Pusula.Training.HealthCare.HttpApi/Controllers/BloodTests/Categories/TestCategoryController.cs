using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.BloodTests.Category;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Categories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("TestCategories")]
    [Route("api/app/bloodtest/test-categories")]
    public class TestCategoryController(ITestCategoryAppService testCategoryAppService) : HealthCareController, ITestCategoryAppService
    {
        [HttpGet]
        [Route("{id}")]
        public Task<TestCategoryDto> GetAsync(Guid id) => testCategoryAppService.GetAsync(id);

        [HttpGet]
        public Task<PagedResultDto<TestCategoryDto>> GetListAsync(GetTestCategoriesInput input) => testCategoryAppService.GetListAsync(input);

        [HttpPost]
        public Task<TestCategoryDto> CreateAsync(TestCategoryCreateDto input) => testCategoryAppService.CreateAsync(input);

        [HttpPut]
        public Task<TestCategoryDto> UpdateAsync(TestCategoryUpdateDto input) => testCategoryAppService.UpdateAsync(input);
    }
}
