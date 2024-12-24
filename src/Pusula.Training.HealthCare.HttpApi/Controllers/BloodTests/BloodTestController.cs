using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.BloodTests
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BloodTests")]
    [Route("api/app/blood-tests")]
    public class BloodTestController(IBloodTestAppService bloodTestAppService) : HealthCareController, IBloodTestAppService
    {
        [HttpGet]
        public Task<PagedResultDto<BloodTestDto>> GetListAsync(GetBloodTestsInput input) => bloodTestAppService.GetListAsync(input);

        [HttpGet]
        [Route("{id}")]
        public Task<BloodTestDto> GetAsync(Guid id) => bloodTestAppService.GetAsync(id);

        [HttpPost]
        public Task<BloodTestDto> CreateAsync(BloodTestCreateDto input) => bloodTestAppService.CreateAsync(input);

        [HttpPut]
        [Route("{id}")]
        public Task<BloodTestDto> UpdateAsync(BloodTestUpdateDto input) => bloodTestAppService.UpdateAsync(input);

        [HttpGet]
        [Route("as-excel-file")]
        public Task<IRemoteStreamContent> GetListAsExcelFileAsync(BloodTestExcelDownloadDto input) => bloodTestAppService.GetListAsExcelFileAsync(input);

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => bloodTestAppService.GetDownloadTokenAsync();

        [HttpGet]
        [Route("{id}/category-ids")]
        public Task<List<Guid>> GetCategoryIdsAsync(Guid id) => bloodTestAppService.GetCategoryIdsAsync(id);
    }
}
