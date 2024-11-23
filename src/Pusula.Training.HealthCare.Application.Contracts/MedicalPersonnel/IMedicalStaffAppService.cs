using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

public interface IMedicalStaffAppService : IApplicationService
{
    Task<PagedResultDto<MedicalStaffWithNavigationPropertiesDto>> GetListAsync(GetMedicalStaffInput input);

    Task<MedicalStaffWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

    Task<MedicalStaffDto> GetAsync(Guid id);

    Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(Guid? cityId, LookupRequestDto input);

    Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);

    Task DeleteAsync(Guid id);

    Task<MedicalStaffDto> CreateAsync(MedicalStaffCreateDto input);

    Task<MedicalStaffDto> UpdateAsync(MedicalStaffUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalStaffExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> medicalStaffIds);

    Task DeleteAllAsync(GetMedicalStaffInput input);
    Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}