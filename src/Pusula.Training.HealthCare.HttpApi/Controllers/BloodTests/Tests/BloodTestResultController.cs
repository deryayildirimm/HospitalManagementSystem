using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Tests;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Tests
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BloodTestResult")]
    [Route("api/app/test/blood-test-results")]
    public class BloodTestResultController(IBloodTestResultAppService bloodTestResultAppService) : HealthCareController, IBloodTestResultAppService
    {
        [HttpGet]
        public Task<PagedResultDto<BloodTestResultWithNavigationPropertiesDto>> GetListAsync(GetBloodTestResultsInput input) => bloodTestResultAppService.GetListAsync(input);

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<BloodTestResultWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => bloodTestResultAppService.GetWithNavigationPropertiesAsync(id);

        [HttpGet]
        [Route("{id}")]
        public Task<BloodTestResultDto> GetAsync(Guid id) => bloodTestResultAppService.GetAsync(id);

        [HttpPost]
        public Task<BloodTestResultDto> CreateAsync(BloodTestResultCreateDto input) => bloodTestResultAppService.CreateAsync(input);

        [HttpPost]
        [Route("generate-results/{bloodTestId}")]
        public Task GenerateResultsForBloodTestAsync(Guid bloodTestId) => bloodTestResultAppService.GenerateResultsForBloodTestAsync(bloodTestId);
        

    }
}
