using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Titles;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Titles.Default)]
public class TitlesAppService(ITitleRepository titleRepository,
    TitleManager titleManager) : HealthCareAppService, ITitlesAppService
{
    public virtual async Task<PagedResultDto<TitleDto>> GetListAsync(GetTitlesInput input)
    {
        var totalCount = await titleRepository.GetCountAsync(input.FilterText, input.TitleName);
        var items = await titleRepository.GetListAsync(input.FilterText, input.TitleName, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<TitleDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Title>, List<TitleDto>>(items)
        };
    }

    public virtual async Task<TitleDto> GetAsync(Guid id)
    {
        var title = await titleRepository.GetAsync(id);
        
        return ObjectMapper.Map<Title, TitleDto>(title);
    }

    [Authorize(HealthCarePermissions.Titles.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await titleRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Titles.Create)]
    public virtual async Task<TitleDto> CreateAsync(TitleCreateDto input)
    {
        var title = await titleManager.CreateAsync(input.TitleName);
        
        return ObjectMapper.Map<Title, TitleDto>(title);
    }

    [Authorize(HealthCarePermissions.Titles.Edit)]
    public virtual async Task<TitleDto> UpdateAsync(TitleUpdateDto input)
    {
        var title = await titleManager.UpdateAsync(input.Id, input.TitleName);
        
        return ObjectMapper.Map<Title, TitleDto>(title);
    }

    [Authorize(HealthCarePermissions.Titles.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> titleIds)
    {
        await titleRepository.DeleteManyAsync(titleIds);
    }

    [Authorize(HealthCarePermissions.Titles.Delete)]
    public virtual async Task DeleteAllAsync(GetTitlesInput input)
    {
        await titleRepository.DeleteAllAsync(input.FilterText, input.TitleName);
    }
}