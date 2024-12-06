using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Treatment.Examinations;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Examinations;

[RemoteService]
[Area("app")]
[ControllerName("Examination")]
[Route("api/app/examinations")]
public class ExaminationController(IExaminationsAppService examinationsAppService) : HealthCareController, IExaminationsAppService
{
    [HttpGet]
    public async Task<PagedResultDto<ExaminationDto>> GetListAsync([FromQuery] GetExaminationsInput input) => await examinationsAppService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<ExaminationDto> GetAsync(Guid id) => examinationsAppService.GetAsync(id);
    
    [HttpPost]
    public Task<ExaminationDto> CreateAsync(ExaminationCreateDto input) => examinationsAppService.CreateAsync(input);
    [HttpPut]
    public Task<ExaminationDto> UpdateAsync(ExaminationUpdateDto input) => examinationsAppService.UpdateAsync(input);
    [HttpDelete]
    [Route("{id}")]
    public void DeleteAsync(Guid id) => examinationsAppService.DeleteAsync(id);
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetExaminationsInput input) => examinationsAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> examinationIds) => examinationsAppService.DeleteByIdsAsync(examinationIds);
}