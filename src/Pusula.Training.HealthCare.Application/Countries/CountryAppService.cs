using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Countries
{
    public class CountryAppService : HealthCareAppService, ICountryAppService
    {
        private readonly string _filePath;

        public CountryAppService(IHostEnvironment env)
        {
            _filePath = Path.Combine(env.ContentRootPath, "..", "Pusula.Training.HealthCare.Domain.Shared", "Countries", "countryPhoneCodes.json");
        }
        public async Task<List<CountryPhoneCodeDto>> GetCountryPhoneCodesAsync()
        {
            var jsonData = await File.ReadAllTextAsync(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Küçük harf ile anahtar isimlerini eşleştirir
            };

            var countryPhoneCodes = JsonSerializer.Deserialize<List<CountryPhoneCodeDto>>(jsonData, options);

            return countryPhoneCodes!;
        }
    }
}