using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Examinations.PhysicalFindings;

[RemoteService]
[Area("app")]
[ControllerName("PhysicalFindings")]
[Route("api/app/physical-findings")]
public class PhysicalFindingController(IPhysicalFindingsAppService physicalFindingsAppService) : HealthCareController, 
    IPhysicalFindingsAppService
{
    [HttpGet]
    public async Task<PagedResultDto<PhysicalFindingDto>> GetListAsync([FromQuery] GetPhysicalFindingsInput input) => 
        await physicalFindingsAppService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<PhysicalFindingDto> GetAsync(Guid id) => physicalFindingsAppService.GetAsync(id);
    
    [HttpPost]
    public Task<PhysicalFindingDto> CreateAsync(PhysicalFindingCreateDto input) => physicalFindingsAppService.CreateAsync(input);
    [HttpPut]
    public Task<PhysicalFindingDto> UpdateAsync(PhysicalFindingUpdateDto input) => physicalFindingsAppService.UpdateAsync(input);
}