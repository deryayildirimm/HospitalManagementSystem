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
    [HttpGet]
    public virtual Task<PagedResultDto<MedicalServiceDto>> GetListAsync(GetMedicalServiceInput input)
        => medicalServicesAppService.GetListAsync(input);

    [HttpGet]
    [Route("by-department")]
    public virtual Task<PagedResultDto<MedicalServiceDto>> GetMedicalServiceByDepartmentIdAsync(
        GetServiceByDepartmentInput input)
        => medicalServicesAppService.GetMedicalServiceByDepartmentIdAsync(input);

    [HttpGet]
    [Route("with-departments")]
    public virtual Task<PagedResultDto<MedicalServiceWithDepartmentsDto>> GetMedicalServiceWithDepartmentsAsync(
        GetMedicalServiceInput input)
        => medicalServicesAppService.GetMedicalServiceWithDepartmentsAsync(input);

    [HttpGet]
    [Route("with-doctors")]
    public virtual Task<MedicalServiceWithDoctorsDto> GetMedicalServiceWithDoctorsAsync(GetMedicalServiceInput input)
        => medicalServicesAppService.GetMedicalServiceWithDoctorsAsync(input);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<MedicalServiceDto> GetAsync(Guid id)
        => medicalServicesAppService.GetAsync(id);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
        => medicalServicesAppService.DeleteAsync(id);

    [HttpPost]
    public virtual Task<MedicalServiceDto> CreateAsync(MedicalServiceCreateDto input)
        => medicalServicesAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public virtual Task<MedicalServiceDto> UpdateAsync(Guid id, MedicalServiceUpdateDto input)
        => medicalServicesAppService.UpdateAsync(id, input);

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalServiceExcelDownloadDto input)
        => medicalServicesAppService.GetListAsExcelFileAsync(input);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> departmentIds) =>
        medicalServicesAppService.DeleteByIdsAsync(departmentIds);

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetMedicalServiceInput input)
        => medicalServicesAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        => medicalServicesAppService.GetDownloadTokenAsync();
}