using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using Pusula.Training.HealthCare.Shared;

namespace Pusula.Training.HealthCare.Patients;

public interface IPatientsAppService : IApplicationService
{
    Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input);

    Task<PatientDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<PatientDto> CreateAsync(PatientCreateDto input);

    Task<PatientDto> UpdateAsync(Guid id, PatientUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(PatientExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> patientIds);

    Task DeleteAllAsync(GetPatientsInput input);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync();
}