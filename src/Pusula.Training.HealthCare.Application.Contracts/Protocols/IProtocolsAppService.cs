using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Protocols;

public interface IProtocolsAppService : IApplicationService
{
    Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input);

    Task<ProtocolWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

    Task<ProtocolDto> GetAsync(Guid id);

    Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);

    Task DeleteAsync(Guid id);

    Task<ProtocolDto> CreateAsync(ProtocolCreateDto input);

    Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> protocolIds);

    Task DeleteAllAsync(GetProtocolsInput input);
    Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

}