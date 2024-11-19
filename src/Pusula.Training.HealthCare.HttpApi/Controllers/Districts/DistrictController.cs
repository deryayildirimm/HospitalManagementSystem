using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Districts;

[RemoteService]
[Area("app")]
[ControllerName("District")]
[Route("api/app/districts")]
public class DistrictController(IDistrictsAppService districtsAppService) : HealthCareController, IDistrictsAppService
{
    
    [HttpGet]
    public Task<PagedResultDto<DistrictWithNavigationPropertiesDto>> GetListAsync(GetDistrictsInput input) => 
        districtsAppService.GetListAsync(input);
    
    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public Task<DistrictWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => 
        districtsAppService.GetWithNavigationPropertiesAsync(id);

    [HttpGet]
    [Route("{id}")]
    public Task<DistrictDto> GetAsync(Guid id) => 
        districtsAppService.GetAsync(id);

    [HttpGet]
    [Route("city-lookup")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input) => 
        districtsAppService.GetCityLookupAsync(input);

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => districtsAppService.DeleteAsync(id);

    [HttpPost]
    public Task<DistrictDto> CreateAsync(DistrictCreateDto input) => districtsAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public Task<DistrictDto> UpdateAsync(DistrictUpdateDto input) => districtsAppService.UpdateAsync(input);

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(DistrictExcelDownloadDto input) => 
        districtsAppService.GetListAsExcelFileAsync(input);

    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> districtIds) => districtsAppService.DeleteByIdsAsync(districtIds);

    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetDistrictsInput input) => districtsAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => districtsAppService.GetDownloadTokenAsync();
}