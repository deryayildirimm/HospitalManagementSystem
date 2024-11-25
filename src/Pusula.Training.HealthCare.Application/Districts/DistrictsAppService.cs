using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Districts;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Districts.Default)]
public class DistrictsAppService(
        IDistrictRepository districtRepository,
        DistrictManager districtManager,
        IDistributedCache<DistrictDownloadTokenCacheItem, string> downloadTokenCache,
        ICityRepository cityRepository) : HealthCareAppService, IDistrictsAppService
{
    public virtual async Task<PagedResultDto<DistrictWithNavigationPropertiesDto>> GetListAsync(GetDistrictsInput input)
    {
        var totalCount = await districtRepository.GetCountAsync(input.FilterText, input.Name, input.CityId);
        var items = await districtRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.CityId, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<DistrictWithNavigationPropertiesDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<DistrictWithNavigationProperties>, List<DistrictWithNavigationPropertiesDto>>(items)
        };
    }

    public virtual async Task<DistrictWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return ObjectMapper.Map<DistrictWithNavigationProperties, DistrictWithNavigationPropertiesDto>
            (await districtRepository.GetWithNavigationPropertiesAsync(id));
    }

    public virtual async Task<DistrictDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<District, DistrictDto>(await districtRepository.GetAsync(id));
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
    {
        var query = (await cityRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name != null && x.Name.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<City>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<City>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    [Authorize(HealthCarePermissions.Districts.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await districtRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Districts.Create)]
    public virtual async Task<DistrictDto> CreateAsync(DistrictCreateDto input)
    {
        if (input.CityId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
        }

        var district = await districtManager.CreateAsync(input.CityId, input.Name);

        return ObjectMapper.Map<District, DistrictDto>(district);
    }

    [Authorize(HealthCarePermissions.Districts.Edit)]
    public virtual async Task<DistrictDto> UpdateAsync(DistrictUpdateDto input)
    {
        if (input.CityId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
        }

        var district = await districtManager.UpdateAsync(
        input.Id, input.CityId, input.Name);

        return ObjectMapper.Map<District, DistrictDto>(district);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DistrictExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var districts = await districtRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.Name, input.CityId);
        var items = districts.Select(item => new
        {

            City = item.City?.Name,
            item.District.Name,

        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Districts.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.Districts.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> districtIds)
    {
        await districtRepository.DeleteManyAsync(districtIds);
    }

    [Authorize(HealthCarePermissions.Districts.Delete)]
    public virtual async Task DeleteAllAsync(GetDistrictsInput input)
    {
        await districtRepository.DeleteAllAsync( input.FilterText, input.Name, input.CityId);
    }
    
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new DistrictDownloadTokenCacheItem { Token = token },
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