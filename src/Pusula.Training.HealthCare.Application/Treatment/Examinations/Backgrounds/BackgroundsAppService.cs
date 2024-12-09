using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Backgrounds.Default)]
public class BackgroundsAppService(IBackgroundRepository backgroundRepository,
    BackgroundManager backgroundManager) : HealthCareAppService, IBackgroundsAppService
{
    public virtual async Task<PagedResultDto<BackgroundDto>> GetListAsync(GetBackgroundsInput input)
    {
        var totalCount = await backgroundRepository.GetCountAsync(input.FilterText, input.Allergies, input.Medications,
            input.Habits, input.ExaminationId);
        var items = await backgroundRepository.GetListAsync(input.FilterText, input.Allergies, input.Medications,
            input.Habits, input.ExaminationId, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<BackgroundDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Background>, List<BackgroundDto>>(items)
        };
    }

    public virtual async Task<BackgroundDto> GetAsync(Guid id)
    {
        var background = await backgroundRepository.GetAsync(id);
        
        return ObjectMapper.Map<Background, BackgroundDto>(background);
    }

    [Authorize(HealthCarePermissions.Backgrounds.Delete)]
    public virtual async void DeleteAsync(Guid id)
    {
        await backgroundRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Backgrounds.Create)]
    public virtual async Task<BackgroundDto> CreateAsync(BackgroundCreateDto input)
    {
        var background = await backgroundManager.CreateAsync(input.ExaminationId, input.Allergies, input.Medications,
            input.Habits);
        
        return ObjectMapper.Map<Background, BackgroundDto>(background);
    }

    [Authorize(HealthCarePermissions.Backgrounds.Edit)]
    public virtual async Task<BackgroundDto> UpdateAsync(BackgroundUpdateDto input)
    {
        var background = await backgroundManager.UpdateAsync(input.Id, input.ExaminationId, input.Allergies, input.Medications,
            input.Habits);
        
        return ObjectMapper.Map<Background, BackgroundDto>(background);
    }

    [Authorize(HealthCarePermissions.Backgrounds.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> backgroundIds)
    {
        await backgroundRepository.DeleteManyAsync(backgroundIds);
    }

    [Authorize(HealthCarePermissions.Backgrounds.Delete)]
    public virtual async Task DeleteAllAsync(GetBackgroundsInput input)
    {
        await backgroundRepository.DeleteAllAsync(input.FilterText, input.Allergies, input.Medications, input.Habits, 
            input.ExaminationId);
    }
}