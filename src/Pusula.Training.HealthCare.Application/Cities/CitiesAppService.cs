using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using DistributedCacheEntryOptions = Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions;

namespace Pusula.Training.HealthCare.Cities;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Cities.Default)]
public class CitiesAppService(ICityRepository cityRepository,
    CityManager cityManager,
IDistributedCache<CityDownloadTokenCacheItem, string> downloadTokenCache) : HealthCareAppService, ICitiesAppService
{
   public virtual async Task<PagedResultDto<CityDto>> GetListAsync(GetCitiesInput input)
    {
        var totalCount = await cityRepository.GetCountAsync(input.FilterText, input.Name);
        var items = await cityRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<CityDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<City>, List<CityDto>>(items)
        };
    }

    public virtual async Task<CityDto> GetAsync(Guid id)
    {
        var city = await cityRepository.GetAsync(id);
        
        return ObjectMapper.Map<City, CityDto>(city);
    }

    [Authorize(HealthCarePermissions.Cities.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await cityRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Cities.Create)]
    public virtual async Task<CityDto> CreateAsync(CityCreateDto input)
    {
        var city = await cityManager.CreateAsync(input.Name);
        
        return ObjectMapper.Map<City, CityDto>(city);
    }

    [Authorize(HealthCarePermissions.Cities.Edit)]
    public virtual async Task<CityDto> UpdateAsync(CityUpdateDto input)
    {
        var city = await cityManager.UpdateAsync(input.Id, input.Name);
        
        return ObjectMapper.Map<City, CityDto>(city);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CityExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var cities = await cityRepository.GetListAsync(
            input.FilterText, input.Name);
        var items = cities.Select(item => new
        {
            item.Name

        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Cities.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.Cities.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> cityIds)
    {
        await cityRepository.DeleteManyAsync(cityIds);
    }

    [Authorize(HealthCarePermissions.Cities.Delete)]
    public virtual async Task DeleteAllAsync(GetCitiesInput input)
    {
        await cityRepository.DeleteAllAsync(input.FilterText, input.Name);
    }
    
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new CityDownloadTokenCacheItem { Token = token },
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