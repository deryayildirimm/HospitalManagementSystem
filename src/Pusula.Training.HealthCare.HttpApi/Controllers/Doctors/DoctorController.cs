using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Doctors;

[RemoteService]
[Area("app")]
[ControllerName("Doctor")]
[Route("api/app/doctors")]
public class DoctorController(IDoctorsAppService doctorsAppService) : HealthCareController, IDoctorsAppService
{
    
    [HttpGet]
    public Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetListAsync(GetDoctorsInput input) => 
        doctorsAppService.GetListAsync(input);
    
    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public Task<DoctorWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => 
        doctorsAppService.GetWithNavigationPropertiesAsync(id);

    [HttpGet]
    [Route("{id}")]
    public Task<DoctorDto> GetAsync(Guid id) => 
        doctorsAppService.GetAsync(id);

    [HttpGet]
    [Route("title-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input) => 
        doctorsAppService.GetTitleLookupAsync(input);

    [HttpGet]
    [Route("department-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input) => 
        doctorsAppService.GetDepartmentLookupAsync(input);

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => doctorsAppService.DeleteAsync(id);

    [HttpPost]
    public Task<DoctorDto> CreateAsync(DoctorCreateDto input) => doctorsAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public Task<DoctorDto> UpdateAsync(DoctorUpdateDto input) => doctorsAppService.UpdateAsync(input);

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorExcelDownloadDto input) => 
        doctorsAppService.GetListAsExcelFileAsync(input);
    
    [HttpGet]
    [Route("by-department-ids")]
    public Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetByDepartmentIdsAsync(GetDoctorsWithDepartmentIdsInput input) => 
        doctorsAppService.GetByDepartmentIdsAsync(input);

    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> doctorIds) => doctorsAppService.DeleteByIdsAsync(doctorIds);

    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetDoctorsInput input) => doctorsAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => doctorsAppService.GetDownloadTokenAsync();
}