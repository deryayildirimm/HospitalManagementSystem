using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Results;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Results
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BloodTestResults")]
    [Route("api/app/test/blood-test-results")]
    public class BloodTestResultController(IBloodTestResultAppService bloodTestResultAppService) : HealthCareController, IBloodTestResultAppService
    {
        [HttpGet]
        public Task<PagedResultDto<BloodTestResultDto>> GetListAsync(GetBloodTestResultsInput input) => bloodTestResultAppService.GetListAsync(input);

        [HttpGet]
        [Route("{id}")]
        public Task<BloodTestResultDto> GetAsync(Guid id) => bloodTestResultAppService.GetAsync(id);

        [HttpPost]
        public Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input) => bloodTestResultAppService.CreateAsync(input);

        [HttpPut]
        [Route("{id}")]
        public Task<BloodTestResultDto> UpdateAsync(BloodTestResultUpdateDto input) => bloodTestResultAppService.UpdateAsync(input);
    }
}
