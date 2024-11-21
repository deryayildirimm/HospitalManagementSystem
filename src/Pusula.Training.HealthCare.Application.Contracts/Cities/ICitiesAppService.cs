using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Cities;

public interface ICitiesAppService
{
    Task<PagedResultDto<CityDto>> GetListAsync(GetCitiesInput input);
    Task<CityDto> GetAsync(Guid id);
    void DeleteAsync(Guid id);
    Task<CityDto> CreateAsync(CityCreateDto input);
    Task<CityDto> UpdateAsync(CityUpdateDto input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(CityExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> cityIds);

    Task DeleteAllAsync(GetCitiesInput input);
    Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}