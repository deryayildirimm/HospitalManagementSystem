using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.ProtocolTypes;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.ProtocolTypes;


[RemoteService]
[Area("app")]
[ControllerName("ProtocolTypes")]
[Route("api/app/protocol-types")]
public class ProtocolTypeController(IProtocolTypesAppService typesAppService): HealthCareController, IProtocolTypesAppService
{
    [HttpGet]
    public virtual Task<PagedResultDto<ProtocolTypeDto>> GetListAsync(GetProtocolTypeInput input) => typesAppService.GetListAsync(input);
    

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ProtocolTypeDto> GetAsync(Guid id) => typesAppService.GetAsync(id);
    
    

    [HttpPost]
    public virtual Task<ProtocolTypeDto> CreateAsync(ProtocolTypeCreateDto input) => typesAppService.CreateAsync(input);
  
    [HttpPut]
    [Route("{id}")]
    public virtual Task<ProtocolTypeDto> UpdateAsync(Guid id, ProtocolTypeUpdateDto input) => typesAppService.UpdateAsync(id, input);
 

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id) => typesAppService.DeleteAsync(id);
  

    
    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync() => typesAppService.GetDownloadTokenAsync();
    
    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolTypeExcelDownloadDto input)
        => typesAppService.GetListAsExcelFileAsync(input);
  
    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> protocolTypesIds) => typesAppService.DeleteByIdsAsync(protocolTypesIds);
    

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetProtocolTypeInput input) => typesAppService.DeleteAllAsync(input);

}