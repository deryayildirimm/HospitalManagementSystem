using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.DoctorLeaves;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Distributed;
using DistributedCacheEntryOptions = Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions;

namespace Pusula.Training.HealthCare.ProtocolTypes;
[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.ProtocolTypes.Default)]
public class ProtocolTypesAppService(IProtocolTypeRepository protocolTypeRepository, IProtocolTypeManager protocolTypeManager,
    IDistributedCache<ProtocolTypeDownloadTokenCacheItem, string> downloadTokenCache,
    IDistributedEventBus distributedEventBus) : HealthCareAppService, IProtocolTypesAppService
{
    public virtual async Task<PagedResultDto<ProtocolTypeDto>> GetListAsync(GetProtocolTypeInput input)
    {
        var totalCount = await protocolTypeRepository.GetCountAsync(input.FilterText, input.Name);
        var items = await protocolTypeRepository.GetListAsync(input.FilterText, input.Name,  input.Sorting, input.MaxResultCount, input.SkipCount );
                
        return new PagedResultDto<ProtocolTypeDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<ProtocolType>, List<ProtocolTypeDto>>(items)
        };
    }

    public virtual async Task<ProtocolTypeDto> GetAsync(Guid id)
    {
        await distributedEventBus.PublishAsync(new DoctorLeaveViewedEto() { Id = id, ViewedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        var protocolType = await protocolTypeRepository.GetAsync(id);
        return ObjectMapper.Map<ProtocolType, ProtocolTypeDto>(protocolType);
    }

    [Authorize(HealthCarePermissions.ProtocolTypes.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await protocolTypeRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.ProtocolTypes.Create)]
    public virtual async Task<ProtocolTypeDto> CreateAsync(ProtocolTypeCreateDto input)
    {
        var type = await protocolTypeManager.CreateAsync(input.Name);
        return ObjectMapper.Map<ProtocolType, ProtocolTypeDto>(type);
    }

    [Authorize(HealthCarePermissions.ProtocolTypes.Edit)]
    public virtual async Task<ProtocolTypeDto> UpdateAsync(Guid id, ProtocolTypeUpdateDto input)
    {
        var type = await protocolTypeManager.UpdateAsync(id,input.Name);
        return ObjectMapper.Map<ProtocolType, ProtocolTypeDto>(type);
    }

    [Authorize(HealthCarePermissions.ProtocolTypes.Delete)]
    public virtual async  Task DeleteByIdsAsync(List<Guid> ids)
    {
        await protocolTypeRepository.DeleteManyAsync(ids);
    }

    [Authorize(HealthCarePermissions.ProtocolTypes.Delete)]
    public virtual async Task DeleteAllAsync(GetProtocolTypeInput input)
    {
        await protocolTypeRepository.DeleteAllAsync(input.FilterText, input.Name);
    }

    #region Excel olarak kullanmak ister miyim karar veremedim, dursun şimdilik 
/*
    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorLeaveExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await repo.GetListAsync(input.FilterText, input.DoctorId, input.StartDateMin, input.StartDateMax, input.EndDateMin, input.EndDateMax, input.Reason);

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DoctorLeave>, List<DoctorLeaveExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Leaves.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
*/

    #endregion
    
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new ProtocolTypeDownloadTokenCacheItem { Token = token },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

        return new Shared.DownloadTokenResultDto
        {
            Token = token
        };
    }
    
    
}