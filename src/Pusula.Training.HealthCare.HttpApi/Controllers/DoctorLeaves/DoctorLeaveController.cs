using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.DoctorLeaves;

[RemoteService]
[Area("app")]
[ControllerName("DoctorLeave")]
[Route("api/app/doctorleave")]
public class DoctorLeaveController(IDoctorLeaveAppService leaveService) : HealthCareController, IDoctorLeaveAppService
{
    [HttpGet]
    public virtual Task<PagedResultDto<DoctorLeaveDto>> GetListAsync(GetDoctorLeaveInput input) => leaveService.GetListAsync(input);


    [HttpGet]
    [Route("{id}")]
    public virtual Task<DoctorLeaveDto> GetAsync(Guid id) => leaveService.GetAsync(id);

    [HttpPost]
    public virtual Task<DoctorLeaveDto> CreateAsync(DoctorLeaveCreateDto input) => leaveService.CreateAsync(input);
 
    [HttpPut]
    [Route("{id}")]
    public virtual Task<DoctorLeaveDto> UpdateAsync(Guid id, DoctorLeaveUpdateDto input) => leaveService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id) => leaveService.DeleteAsync(id);
  

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorLeaveExcelDownloadDto input) => leaveService.GetListAsExcelFileAsync(input);
  
    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync() => leaveService.GetDownloadTokenAsync();
  

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> leaveIds) => leaveService.DeleteByIdsAsync(leaveIds);
 
    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetDoctorLeaveInput input) => leaveService.DeleteAllAsync(input);
    
}