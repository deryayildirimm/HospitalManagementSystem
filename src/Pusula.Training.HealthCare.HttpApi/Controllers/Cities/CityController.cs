using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Cities;

[RemoteService]
[Area("app")]
[ControllerName("City")]
[Route("api/app/cities")]
public class CityController(ICitiesAppService citiesAppService) : HealthCareController, ICitiesAppService
{
    [HttpGet]
    public async Task<PagedResultDto<CityDto>> GetListAsync([FromQuery] GetCitiesInput input) =>
        await citiesAppService.GetListAsync(input);
    
    [HttpGet("{id}")]
    public Task<CityDto> GetAsync(Guid id) => citiesAppService.GetAsync(id);
    
    [HttpPost]
    public Task<CityDto> CreateAsync(CityCreateDto input) => citiesAppService.CreateAsync(input);
    
    [HttpPut]
    public Task<CityDto> UpdateAsync(CityUpdateDto input) => citiesAppService.UpdateAsync(input);
    
    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(CityExcelDownloadDto input) => 
        citiesAppService.GetListAsExcelFileAsync(input);
    
    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => citiesAppService.DeleteAsync(id);
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetCitiesInput input) => citiesAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> cityIds) => citiesAppService.DeleteByIdsAsync(cityIds);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => citiesAppService.GetDownloadTokenAsync();
}