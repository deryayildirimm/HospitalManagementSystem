using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.MedicalPersonnel;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.MedicalPersonnel;

[RemoteService]
[Area("app")]
[ControllerName("MedicalStaff")]
[Route("api/app/medical-staff")]
public class MedicalStaffController(IMedicalStaffAppService medicalStaffAppService) : HealthCareController, IMedicalStaffAppService
{
    [HttpGet]
    public Task<PagedResultDto<MedicalStaffWithNavigationPropertiesDto>> GetListAsync(GetMedicalStaffInput input) => 
        medicalStaffAppService.GetListAsync(input);
    
    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public Task<MedicalStaffWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => 
        medicalStaffAppService.GetWithNavigationPropertiesAsync(id);

    [HttpGet]
    [Route("{id}")]
    public Task<MedicalStaffDto> GetAsync(Guid id) => 
        medicalStaffAppService.GetAsync(id);

    [HttpGet]
    [Route("city-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input) => 
        medicalStaffAppService.GetCityLookupAsync(input);

    [HttpGet]
    [Route("district-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(Guid? cityId, LookupRequestDto input) => 
        medicalStaffAppService.GetDistrictLookupAsync(cityId, input);

    [HttpGet]
    [Route("department-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input) => 
        medicalStaffAppService.GetDepartmentLookupAsync(input);

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => medicalStaffAppService.DeleteAsync(id);

    [HttpPost]
    public Task<MedicalStaffDto> CreateAsync(MedicalStaffCreateDto input) => medicalStaffAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public Task<MedicalStaffDto> UpdateAsync(MedicalStaffUpdateDto input) => medicalStaffAppService.UpdateAsync(input);

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalStaffExcelDownloadDto input) => 
        medicalStaffAppService.GetListAsExcelFileAsync(input);

    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> medicalStaffIds) => medicalStaffAppService.DeleteByIdsAsync(medicalStaffIds);

    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetMedicalStaffInput input) => medicalStaffAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => medicalStaffAppService.GetDownloadTokenAsync();
}