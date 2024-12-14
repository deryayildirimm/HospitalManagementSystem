using Pusula.Training.HealthCare.Shared;
using System.Collections.Generic;

namespace Pusula.Training.HealthCare.Countries
{
    public class GetCountryListCacheItem : BaseTokenCacheItem
    {
        public List<CountryPhoneCodeDto> CountryPhoneCodes { get; set; } = null!;
    }
}
