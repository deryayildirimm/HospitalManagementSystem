using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Treatment.Icds;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Icds.Default)]
public class IcdsAppService(
        IIcdRepository icdRepository,
        IcdManager icdManager,
        IDistributedCache<IcdDownloadTokenCacheItem, string> downloadTokenCache
        ) : HealthCareAppService, IIcdsAppService
{
    public virtual async Task<PagedResultDto<IcdDto>> GetListAsync(GetIcdsInput input)
    {
        var totalCount = await icdRepository.GetCountAsync(input.FilterText, input.CodeNumber, input.Detail);
        var items = await icdRepository.GetListAsync(input.FilterText, input.CodeNumber, input.Detail, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<IcdDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Icd>, List<IcdDto>>(items)
        };
    }

    public virtual async Task<IcdDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Icd, IcdDto>(await icdRepository.GetAsync(id));
    }

    [Authorize(HealthCarePermissions.Icds.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await icdRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Icds.Create)]
    public virtual async Task<IcdDto> CreateAsync(IcdCreateDto input)
    {
        var icd = await icdManager.CreateAsync(
            input.CodeNumber,input.Detail
        );

        return ObjectMapper.Map<Icd, IcdDto>(icd);
    }

    [Authorize(HealthCarePermissions.Icds.Edit)]
    public virtual async Task<IcdDto> UpdateAsync(IcdUpdateDto input)
    {
        var icd = await icdManager.UpdateAsync(
        input.Id, input.CodeNumber,input.Detail);

        return ObjectMapper.Map<Icd, IcdDto>(icd);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(IcdExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var icds = await icdRepository.GetListAsync(
            input.FilterText, input.CodeNumber, input.Detail);
        var items = icds.Select(item => new
        {
            item.CodeNumber
        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Icds.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.Icds.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> icdIds)
    {
        await icdRepository.DeleteManyAsync(icdIds);
    }

    [Authorize(HealthCarePermissions.Icds.Delete)]
    public virtual async Task DeleteAllAsync(GetIcdsInput input)
    {
        await icdRepository.DeleteAllAsync( input.FilterText, input.CodeNumber, input.Detail);
    }
    
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new IcdDownloadTokenCacheItem { Token = token },
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