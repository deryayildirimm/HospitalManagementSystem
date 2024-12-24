using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Tests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Tests
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Tests")]
    [Route("api/app/tests")]
    public class TestController(ITestAppService testAppService) : HealthCareController, ITestAppService
    {
        [HttpGet]
        public Task<PagedResultDto<TestDto>> GetListAsync(GetTestsInput input) => testAppService.GetListAsync(input);

        [HttpGet]
        [Route("{id}")]
        public Task<TestDto> GetAsync(Guid id) => testAppService.GetAsync(id);

        [HttpGet]
        [Route("filter-by-categories")]
        public Task<List<TestDto>> GetListByCategoriesAsync([FromQuery] List<Guid> categoryIds) => testAppService.GetListByCategoriesAsync(categoryIds);

        [HttpPost]
        public Task<TestDto> CreateAsync(TestCreateDto input) => testAppService.CreateAsync(input);

        [HttpPut]
        public Task<TestDto> UpdateAsync(TestUpdateDto input) => testAppService.UpdateAsync(input); 
    }
}
