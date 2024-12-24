using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.ProtocolTypes;

public interface IProtocolTypesAppService : IApplicationService
{
    Task<PagedResultDto<ProtocolTypeDto>> GetListAsync(GetProtocolTypeInput input);
    Task<ProtocolTypeDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<ProtocolTypeDto> CreateAsync(ProtocolTypeCreateDto input);
    Task<ProtocolTypeDto> UpdateAsync(Guid id, ProtocolTypeUpdateDto input);
    Task DeleteByIdsAsync(List<Guid> ids);
    Task DeleteAllAsync(GetProtocolTypeInput input);
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolTypeExcelDownloadDto input);
    Task<DownloadTokenResultDto> GetDownloadTokenAsync();
}