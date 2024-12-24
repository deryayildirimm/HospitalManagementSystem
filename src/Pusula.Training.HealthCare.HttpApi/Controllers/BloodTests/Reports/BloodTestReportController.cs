using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.BloodTests.Reports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.BloodTests.Reports
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BloodTestReports")]
    [Route("api/app/blood-test-reports")]
    public class BloodTestReportController(IBloodTestReportAppService bloodTestReportAppService) : HealthCareController, IBloodTestReportAppService
    {
        [HttpGet]
        public Task<PagedResultDto<BloodTestReportDto>> GetListAsync(GetBloodTestReportsInput input) => bloodTestReportAppService.GetListAsync(input);

        [HttpGet]
        [Route("{id}")]
        public Task<BloodTestReportDto> GetAsync(Guid id) => bloodTestReportAppService.GetAsync(id);

        [HttpPost]
        public Task<BloodTestReportDto> CreateAsync(BloodTestReportCreateDto input) => bloodTestReportAppService.CreateAsync(input);

        [HttpPut]
        [Route("{id}")]
        public Task<BloodTestReportDto> UpdateAsync(BloodTestReportUpdateDto input) => bloodTestReportAppService.UpdateAsync(input);

        [HttpGet]
        [Route("{id}/by-blood-test")]
        public Task<BloodTestReportDto> GetByBloodTestIdAsync(Guid id) => bloodTestReportAppService.GetByBloodTestIdAsync(id);

        [HttpGet]
        [Route("by-patient-number/{patientNumber:int}")]
        public Task<List<BloodTestReportDto>> GetListByPatientNumberAsync(int patientNumber) => bloodTestReportAppService.GetListByPatientNumberAsync((int) patientNumber);

        [HttpGet]
        [Route("filtered-results/{patientId}/{testId}")]
        public Task<List<BloodTestReportResultDto>> GetFilteredResultsByPatientAndTestAsync(Guid patientId, Guid testId, CancellationToken cancellationToken = default)
                => bloodTestReportAppService.GetFilteredResultsByPatientAndTestAsync(patientId, testId);
    }
}
