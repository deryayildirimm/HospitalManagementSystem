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
    Task<ProtocolDto> GetWithNavigationPropertiesAsync(Guid id);

    Task<ProtocolDto> GetAsync(Guid id);

    Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<DepartmentStatisticDto>> GetDepartmentPatientStatisticsAsync(GetProtocolsInput input);
    
    Task<PagedResultDto<DoctorStatisticDto>> GetDoctorPatientStatisticsAsync(GetProtocolsInput input);
    
    Task<PagedResultDto<ProtocolPatientDepartmentListReportDto>> GetPatientsByDepartmentAsync(GetProtocolsInput input);
    
    Task<PagedResultDto<ProtocolPatientDoctorListReportDto>> GetPatientsByDoctorAsync(GetProtocolsInput input);

    Task<ProtocolWithDetailsDto> GetProtocolDetailsAsync(Guid id);
    
    
    Task DeleteAsync(Guid id);

    Task<ProtocolDto> CreateAsync(ProtocolCreateDto input);

    Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> protocolIds);

    Task DeleteAllAsync(GetProtocolsInput input);
    Task<DownloadTokenResultDto> GetDownloadTokenAsync();

}