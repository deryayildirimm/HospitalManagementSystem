using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.Countries
{
    //Created by Anıl Oğuzman
    public interface ICountryAppService : IApplicationService
    {
        Task<List<CountryPhoneCodeDto>> GetCountryPhoneCodesAsync();
    }
}