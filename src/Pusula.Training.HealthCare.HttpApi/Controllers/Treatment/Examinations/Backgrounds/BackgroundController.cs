using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Examinations.Backgrounds;

[RemoteService]
[Area("app")]
[ControllerName("Background")]
[Route("api/app/backgrounds")]
public class BackgroundController(IBackgroundsAppService backgroundsAppService) : HealthCareController, IBackgroundsAppService
{
    [HttpGet]
    public async Task<PagedResultDto<BackgroundDto>> GetListAsync([FromQuery] GetBackgroundsInput input) => await backgroundsAppService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<BackgroundDto> GetAsync(Guid id) => backgroundsAppService.GetAsync(id);
    
    [HttpPost]
    public Task<BackgroundDto> CreateAsync(BackgroundCreateDto input) => backgroundsAppService.CreateAsync(input);
    [HttpPut]
    public Task<BackgroundDto> UpdateAsync(BackgroundUpdateDto input) => backgroundsAppService.UpdateAsync(input);
}