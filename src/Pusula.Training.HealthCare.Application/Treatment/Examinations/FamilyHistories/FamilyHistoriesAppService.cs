using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.FamilyHistories.Default)]
public class FamilyHistoriesAppService(IFamilyHistoryRepository familyHistoryRepository,
    FamilyHistoryManager familyHistoryManager) : HealthCareAppService, IFamilyHistoriesAppService
{
    public virtual async Task<PagedResultDto<FamilyHistoryDto>> GetListAsync(GetFamilyHistoriesInput input)
    {
        var totalCount = await familyHistoryRepository.GetCountAsync(input.FilterText, input.MotherDisease, input.FatherDisease,
            input.SisterDisease, input.BrotherDisease, input.AreParentsRelated, input.ExaminationId);
        var items = await familyHistoryRepository.GetListAsync(input.FilterText, input.MotherDisease, 
            input.FatherDisease, input.SisterDisease, input.BrotherDisease, input.AreParentsRelated, input.ExaminationId, 
            input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<FamilyHistoryDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<FamilyHistory>, List<FamilyHistoryDto>>(items)
        };
    }

    public virtual async Task<FamilyHistoryDto> GetAsync(Guid id)
    {
        var familyHistory = await familyHistoryRepository.GetAsync(id);
        
        return ObjectMapper.Map<FamilyHistory, FamilyHistoryDto>(familyHistory);
    }

    [Authorize(HealthCarePermissions.FamilyHistories.Create)]
    public virtual async Task<FamilyHistoryDto> CreateAsync(FamilyHistoryCreateDto input)
    {
        var familyHistory = await familyHistoryManager.CreateAsync(input.ExaminationId, input.AreParentsRelated, 
            input.MotherDisease, input.FatherDisease, input.SisterDisease, input.BrotherDisease);
        
        return ObjectMapper.Map<FamilyHistory, FamilyHistoryDto>(familyHistory);
    }

    [Authorize(HealthCarePermissions.FamilyHistories.Edit)]
    public virtual async Task<FamilyHistoryDto> UpdateAsync(FamilyHistoryUpdateDto input)
    {
        var familyHistory = await familyHistoryManager.UpdateAsync(input.Id, input.ExaminationId, input.AreParentsRelated, 
            input.MotherDisease, input.FatherDisease, input.SisterDisease, input.BrotherDisease);
        
        return ObjectMapper.Map<FamilyHistory, FamilyHistoryDto>(familyHistory);
    }
}