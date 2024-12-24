using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Restrictions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Restrictions;

[RemoteService]
[Area("app")]
[ControllerName("Restriction")]
[Route("api/app/restrictions")]
public class RestrictionController(IRestrictionAppService restrictionAppService)
    : HealthCareController, IRestrictionAppService
{
    [HttpGet]
    public Task<PagedResultDto<RestrictionDto>> GetListAsync(GetRestrictionsInput input)
        => restrictionAppService.GetListAsync(input);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<RestrictionDto> GetAsync(Guid id)
        => restrictionAppService.GetAsync(id);

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(RestrictionExcelDownloadDto input)
        => restrictionAppService.GetListAsExcelFileAsync(input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
        => restrictionAppService.DeleteAsync(id);

    [HttpPost]
    public virtual Task<RestrictionDto> CreateAsync(RestrictionCreateDto input)
        => restrictionAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public virtual Task<RestrictionDto> UpdateAsync(Guid id, RestrictionUpdateDto input)
        => restrictionAppService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> ids)
        => restrictionAppService.DeleteByIdsAsync(ids);

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        => restrictionAppService.GetDownloadTokenAsync();
}