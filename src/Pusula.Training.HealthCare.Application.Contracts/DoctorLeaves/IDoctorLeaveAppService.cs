using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.DoctorLeaves;

public interface IDoctorLeaveAppService: IApplicationService
{
    Task<PagedResultDto<DoctorLeaveDto>> GetListAsync(GetDoctorLeaveInput input);

    Task<DoctorLeaveDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<DoctorLeaveDto> CreateAsync(DoctorLeaveCreateDto input);

    Task<DoctorLeaveDto> UpdateAsync(Guid id, DoctorLeaveUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorLeaveExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> leaveIds);

    Task DeleteAllAsync(GetDoctorLeaveInput input);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync();
}