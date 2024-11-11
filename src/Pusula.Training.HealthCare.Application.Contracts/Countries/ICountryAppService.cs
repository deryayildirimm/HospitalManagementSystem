using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.Countries
{
    public interface ICountryAppService : IApplicationService
    {
        Task<List<CountryPhoneCodeDto>> GetCountryPhoneCodesAsync();
    }
}