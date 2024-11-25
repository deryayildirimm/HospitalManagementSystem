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
    [Route("api/app/bloodtests")]
    public class BloodTestController(IBloodTestAppService bloodTestAppService) : HealthCareController, IBloodTestAppService
    {
        [HttpGet]
        public Task<PagedResultDto<BloodTestWithNavigationPropertiesDto>> GetListAsync(GetBloodTestsInput input) => bloodTestAppService.GetListAsync(input);

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<BloodTestWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => bloodTestAppService.GetWithNavigationPropertiesAsync(id);

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

        [HttpPost]
        [Route("api/app/blood-tests/bulk-update-status")]
        public Task BulkUpdateStatusAsync(List<BloodTestUpdateDto> updateDtos) => bloodTestAppService.BulkUpdateStatusAsync(updateDtos);
    }
}
