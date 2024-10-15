using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Protocols;

[RemoteService]
[Area("app")]
[ControllerName("Protocol")]
[Route("api/app/protocols")]
public class ProtocolController : HealthCareController, IProtocolsAppService
{
    protected IProtocolsAppService _protocolsAppService;

    public ProtocolController(IProtocolsAppService protocolsAppService)
    {
        _protocolsAppService = protocolsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input)
    {
        return _protocolsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public virtual Task<ProtocolWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return _protocolsAppService.GetWithNavigationPropertiesAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ProtocolDto> GetAsync(Guid id)
    {
        return _protocolsAppService.GetAsync(id);
    }

    [HttpGet]
    [Route("patient-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input)
    {
        return _protocolsAppService.GetPatientLookupAsync(input);
    }

    [HttpGet]
    [Route("department-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
    {
        return _protocolsAppService.GetDepartmentLookupAsync(input);
    }

    [HttpPost]
    public virtual Task<ProtocolDto> CreateAsync(ProtocolCreateDto input)
    {
        return _protocolsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input)
    {
        return _protocolsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _protocolsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input)
    {
        return _protocolsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _protocolsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> protocolIds)
    {
        return _protocolsAppService.DeleteByIdsAsync(protocolIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetProtocolsInput input)
    {
        return _protocolsAppService.DeleteAllAsync(input);
    }
}