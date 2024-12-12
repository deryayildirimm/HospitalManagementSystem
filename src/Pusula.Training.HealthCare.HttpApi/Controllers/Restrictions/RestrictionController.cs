using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Restrictions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

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
    public Task<RestrictionDto> GetAsync(Guid id)
        => restrictionAppService.GetAsync(id);

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
        => restrictionAppService.DeleteAsync(id);

    [HttpPost]
    public Task<RestrictionDto> CreateAsync(RestrictionCreateDto input)
        => restrictionAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public Task<RestrictionDto> UpdateAsync(Guid id, RestrictionUpdateDto input)
        => restrictionAppService.UpdateAsync(id, input);
    
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> ids)
        => restrictionAppService.DeleteByIdsAsync(ids);
}