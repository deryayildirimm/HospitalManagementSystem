using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Treatment.Examinations.FamilyHistories;

[RemoteService]
[Area("app")]
[ControllerName("FamilyHistories")]
[Route("api/app/family-histories")]
public class FamilyHistoryController(IFamilyHistoriesAppService familyHistoriesAppService) : HealthCareController, IFamilyHistoriesAppService
{
    [HttpGet]
    public async Task<PagedResultDto<FamilyHistoryDto>> GetListAsync([FromQuery] GetFamilyHistoriesInput input) => await familyHistoriesAppService.GetListAsync(input);

    [HttpGet("{id}")]
    public Task<FamilyHistoryDto> GetAsync(Guid id) => familyHistoriesAppService.GetAsync(id);
    
    [HttpPost]
    public Task<FamilyHistoryDto> CreateAsync(FamilyHistoryCreateDto input) => familyHistoriesAppService.CreateAsync(input);
    [HttpPut]
    public Task<FamilyHistoryDto> UpdateAsync(FamilyHistoryUpdateDto input) => familyHistoriesAppService.UpdateAsync(input);
    [HttpDelete]
    [Route("{id}")]
    public void DeleteAsync(Guid id) => familyHistoriesAppService.DeleteAsync(id);
    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetFamilyHistoriesInput input) => familyHistoriesAppService.DeleteAllAsync(input);
    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> familyHistoryIds) => familyHistoriesAppService.DeleteByIdsAsync(familyHistoryIds);
}