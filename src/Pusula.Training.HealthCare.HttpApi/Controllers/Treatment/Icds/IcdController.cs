using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Shared;
using Pusula.Training.HealthCare.Treatment.Icds;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Icds;

public class IcdController(IIcdAppService icdAppService) : HealthCareController, IIcdAppService
{
    [HttpGet]
    public async Task<PagedResultDto<IcdDto>> GetListAsync([FromQuery] GetIcdsInput input) =>
        await icdAppService.GetListAsync(input);
    
    [HttpGet("{id}")]
    public Task<IcdDto> GetAsync(Guid id) => icdAppService.GetAsync(id);
    
    [HttpPost]
    public Task<IcdDto> CreateAsync(IcdCreateDto input) => icdAppService.CreateAsync(input);
    
    [HttpPut]
    public Task<IcdDto> UpdateAsync(IcdUpdateDto input) => icdAppService.UpdateAsync(input);
    
    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(IcdExcelDownloadDto input) => 
        icdAppService.GetListAsExcelFileAsync(input);
    
    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id) => icdAppService.DeleteAsync(id);
    
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetIcdsInput input) => icdAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> icdIds) => icdAppService.DeleteByIdsAsync(icdIds);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => icdAppService.GetDownloadTokenAsync();
}