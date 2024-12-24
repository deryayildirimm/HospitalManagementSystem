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
public class ProtocolController(IProtocolsAppService protocolsAppService) : HealthCareController, IProtocolsAppService
{

    [HttpGet]
    public virtual Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input) => protocolsAppService.GetListAsync(input);
   

    [HttpGet]
    [Route("with-navigation-properties/{id}")]
    public virtual Task<ProtocolDto> GetWithNavigationPropertiesAsync(Guid id) => protocolsAppService.GetWithNavigationPropertiesAsync(id);
    

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ProtocolDto> GetAsync(Guid id) => protocolsAppService.GetAsync(id);
   
    
    [HttpGet]
    [Route("department-statistic")]
    public Task<PagedResultDto<DepartmentStatisticDto>> GetDepartmentPatientStatisticsAsync(GetProtocolsInput input)
        => protocolsAppService.GetDepartmentPatientStatisticsAsync(input);
    
    [HttpGet]
    [Route("doctor-statistic")]
    public Task<PagedResultDto<DoctorStatisticDto>> GetDoctorPatientStatisticsAsync(GetProtocolsInput input)
        => protocolsAppService.GetDoctorPatientStatisticsAsync(input);
    
    [HttpGet]
    [Route("patient-list-by-department")]
    public Task<PagedResultDto<ProtocolPatientDepartmentListReportDto>> GetPatientsByDepartmentAsync(GetProtocolsInput input)
        => protocolsAppService.GetPatientsByDepartmentAsync(input);
    
    [HttpGet]
    [Route("patient-list-by-doctor")]
    public Task<PagedResultDto<ProtocolPatientDoctorListReportDto>> GetPatientsByDoctorAsync(GetProtocolsInput input)
        => protocolsAppService.GetPatientsByDoctorAsync(input);

    [HttpGet]
    [Route("patient-lookup")]
    public virtual Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input) => protocolsAppService.GetPatientLookupAsync(input);
    
    
    [HttpGet]
    [Route("protocol-detail")]
    public virtual Task<ProtocolWithDetailsDto> GetProtocolDetailsAsync(Guid id) => protocolsAppService.GetProtocolDetailsAsync(id);

    
    [HttpPost]
    public virtual Task<ProtocolDto> CreateAsync(ProtocolCreateDto input) => protocolsAppService.CreateAsync(input);
    

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input) => protocolsAppService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id) => protocolsAppService.DeleteAsync(id);
   

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input) => protocolsAppService.GetListAsExcelFileAsync(input);
  
    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync() => protocolsAppService.GetDownloadTokenAsync();
  

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> protocolIds) => protocolsAppService.DeleteByIdsAsync(protocolIds);
    

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetProtocolsInput input) => protocolsAppService.DeleteAllAsync(input);
   
}