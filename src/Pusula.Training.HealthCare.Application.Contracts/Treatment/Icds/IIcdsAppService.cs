using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Treatment.Icds;

public interface IIcdsAppService : IApplicationService
{
    Task<PagedResultDto<IcdDto>> GetListAsync(GetIcdsInput input);

    Task<IcdDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);

    Task<IcdDto> CreateAsync(IcdCreateDto input);

    Task<IcdDto> UpdateAsync(IcdUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(IcdExcelDownloadDto input);
    
    Task DeleteByIdsAsync(List<Guid> icdIds);

    Task DeleteAllAsync(GetIcdsInput input);
    Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}