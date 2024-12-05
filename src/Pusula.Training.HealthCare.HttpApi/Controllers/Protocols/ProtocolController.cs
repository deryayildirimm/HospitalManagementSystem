using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Protocols;

[RemoteService]
[Area("app")]
[ControllerName("Protocol")]
[Route("api/app/protocols")]
public class ProtocolController(IProtocolsAppService _protocolsAppService) : HealthCareController, IProtocolsAppService
{

    [HttpGet]
    public virtual Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input) => _protocolsAppService.GetListAsync(input);
   

    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public virtual Task<ProtocolWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => _protocolsAppService.GetWithNavigationPropertiesAsync(id);
    

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ProtocolDto> GetAsync(Guid id) => _protocolsAppService.GetAsync(id);
   

    [HttpGet]
    [Route("patient-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input) => _protocolsAppService.GetPatientLookupAsync(input);
  
    [HttpGet]
    [Route("department-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input) => _protocolsAppService.GetDepartmentLookupAsync(input);
    
    [HttpGet]
    [Route("doctor-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetDoctorLookUpAsync(LookupRequestDto input) => _protocolsAppService.GetDoctorLookUpAsync(input);
    
    [HttpGet]
    [Route("protocol-type-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProtocolTypeLookUpAsync(LookupRequestDto input) => _protocolsAppService.GetProtocolTypeLookUpAsync(input);


    [HttpPost]
    public virtual Task<ProtocolDto> CreateAsync(ProtocolCreateDto input) => _protocolsAppService.CreateAsync(input);
    

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input) => _protocolsAppService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id) => _protocolsAppService.DeleteAsync(id);
   

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input) => _protocolsAppService.GetListAsExcelFileAsync(input);
  
    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync() => _protocolsAppService.GetDownloadTokenAsync();
  

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> protocolIds) => _protocolsAppService.DeleteByIdsAsync(protocolIds);
    

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetProtocolsInput input) => _protocolsAppService.DeleteAllAsync(input);
   
}