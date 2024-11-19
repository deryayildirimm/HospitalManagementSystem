using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Doctors;

public interface IDoctorsAppService : IApplicationService
{
    Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetListAsync(GetDoctorsInput input);

    Task<DoctorWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

    Task<DoctorDto> GetAsync(Guid id);

    Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);
    Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetByDepartmentIdsAsync(GetDoctorsWithDepartmentIdsInput input);

    Task DeleteAsync(Guid id);

    Task<DoctorDto> CreateAsync(DoctorCreateDto input);

    Task<DoctorDto> UpdateAsync(DoctorUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> doctorIds);

    Task DeleteAllAsync(GetDoctorsInput input);
    Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}