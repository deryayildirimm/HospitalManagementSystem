using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Pusula.Training.HealthCare.Countries
{
    public class CountryAppService : HealthCareAppService, ICountryAppService
    {
        private readonly string _filePath;
        private readonly IDistributedCache<GetCountryListCacheItem> _getCountryListCache;

        public CountryAppService(IHostEnvironment env, IDistributedCache<GetCountryListCacheItem> getCountryListCache)
        {
            _filePath = Path.Combine(env.ContentRootPath, "..", "Pusula.Training.HealthCare.Domain.Shared", "Countries", "countryPhoneCodes.json");
            _getCountryListCache = getCountryListCache;
        }

        public async Task<List<CountryPhoneCodeDto>> GetCountryPhoneCodesAsync()
        {
            var token = "GetCountryListService";
            var cacheResult = await _getCountryListCache.GetAsync(token);

            if (cacheResult != null)
                return cacheResult.CountryPhoneCodes;

            var jsonData = await File.ReadAllTextAsync(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            var countryPhoneCodes = JsonSerializer.Deserialize<List<CountryPhoneCodeDto>>(jsonData, options);
            
            await _getCountryListCache.SetAsync(token,
                new GetCountryListCacheItem
                {
                    CountryPhoneCodes = countryPhoneCodes!
                },
                new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromDays(1) // Servis kullanıldığında süreyi 1 gün uzat
                });

            return countryPhoneCodes!;
        }

        public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _getCountryListCache.SetAsync(
                token,
                new GetCountryListCacheItem { Token = token },
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
}