using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Districts;

public interface IDistrictsAppService : IApplicationService
{
    Task<PagedResultDto<DistrictWithNavigationPropertiesDto>> GetListAsync(GetDistrictsInput input);

    Task<DistrictWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

    Task<DistrictDto> GetAsync(Guid id);

    Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input);

    Task DeleteAsync(Guid id);

    Task<DistrictDto> CreateAsync(DistrictCreateDto input);

    Task<DistrictDto> UpdateAsync(DistrictUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(DistrictExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> districtIds);

    Task DeleteAllAsync(GetDistrictsInput input);
    Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}