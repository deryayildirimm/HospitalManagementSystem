using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Titles;

[RemoteService]
[Area("app")]
[ControllerName("Title")]
[Route("api/app/titles")]
public class TitleController(ITitlesAppService titlesAppService) : HealthCareController, ITitlesAppService
{
    [HttpGet]
    public async Task<PagedResultDto<TitleDto>> GetListAsync([FromQuery] GetTitlesInput input)
    {
        Console.Write("ALOOOOOO");
        return await titlesAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<TitleDto> GetAsync(Guid id)
    {
        Console.Write("Id ile Get");
        return titlesAppService.GetAsync(id);
    }
    
    [HttpPost]
    public Task<TitleDto> CreateAsync(TitleCreateDto input) => titlesAppService.CreateAsync(input);
    [HttpPut]
    public Task<TitleDto> UpdateAsync(TitleUpdateDto input) => titlesAppService.UpdateAsync(input);
    [HttpDelete]
    [Route("{id}")]
    public void DeleteAsync(Guid id) => titlesAppService.DeleteAsync(id);
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetTitlesInput input) => titlesAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> titleIds) => titlesAppService.DeleteByIdsAsync(titleIds);
}