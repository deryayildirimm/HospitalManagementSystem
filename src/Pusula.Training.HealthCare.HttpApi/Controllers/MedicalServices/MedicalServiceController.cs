using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.MedicalServices;

[RemoteService]
[Area("app")]
[ControllerName("MedicalService")]
[Route("api/app/medical-service")]

public class MedicalServiceController(IMedicalServicesAppService medicalServicesAppService)
    : HealthCareController, IMedicalServicesAppService
{
    protected IMedicalServicesAppService medicalServicesAppService = medicalServicesAppService;

    [HttpGet]
    public Task<PagedResultDto<MedicalServiceDto>> GetListAsync(GetMedicalServiceInput input) =>
        medicalServicesAppService.GetListAsync(input);

    [HttpGet]
    [Route("department-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input) => medicalServicesAppService.GetDepartmentLookupAsync(input);
    
    [HttpGet]
    [Route("{id}")]
    public Task<MedicalServiceDto> GetAsync(Guid id) => medicalServicesAppService.GetAsync(id);

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => medicalServicesAppService.DeleteAsync(id);

    [HttpPost]
    public Task<MedicalServiceDto> CreateAsync(MedicalServiceCreateDto input) =>
        medicalServicesAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public Task<MedicalServiceDto> UpdateAsync(Guid id, MedicalServiceUpdateDto input) =>
        medicalServicesAppService.UpdateAsync(id, input);

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalServiceExcelDownloadDto input) =>
        medicalServicesAppService.GetListAsExcelFileAsync(input);

    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> departmentIds) =>
        medicalServicesAppService.DeleteByIdsAsync(departmentIds);

    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetMedicalServiceInput input) => medicalServicesAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => medicalServicesAppService.GetDownloadTokenAsync();
}