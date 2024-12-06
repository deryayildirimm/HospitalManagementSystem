using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Shared;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Icds;

[RemoteService]
[Area("app")]
[ControllerName("Icd")]
[Route("api/app/icds")]
public class IcdController(IIcdsAppService icdsAppService) : HealthCareController, IIcdsAppService
{
    [HttpGet]
    public async Task<PagedResultDto<IcdDto>> GetListAsync([FromQuery] GetIcdsInput input) =>
        await icdsAppService.GetListAsync(input);
    
    [HttpGet("{id}")]
    public Task<IcdDto> GetAsync(Guid id) => icdsAppService.GetAsync(id);
    
    [HttpPost]
    public Task<IcdDto> CreateAsync(IcdCreateDto input) => icdsAppService.CreateAsync(input);
    
    [HttpPut]
    public Task<IcdDto> UpdateAsync(IcdUpdateDto input) => icdsAppService.UpdateAsync(input);
    
    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(IcdExcelDownloadDto input) => 
        icdsAppService.GetListAsExcelFileAsync(input);
    
    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => icdsAppService.DeleteAsync(id);
    
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetIcdsInput input) => icdsAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> icdIds) => icdsAppService.DeleteByIdsAsync(icdIds);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => icdsAppService.GetDownloadTokenAsync();
}