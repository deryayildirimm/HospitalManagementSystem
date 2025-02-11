using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.MedicalServices;

public interface IMedicalServicesAppService : IApplicationService
{
    Task<PagedResultDto<MedicalServiceDto>> GetListAsync(GetMedicalServiceInput input);

    Task<PagedResultDto<MedicalServiceDto>> GetMedicalServiceByDepartmentIdAsync(
        GetServiceByDepartmentInput input);
    
    Task<PagedResultDto<MedicalServiceWithDepartmentsDto>> GetMedicalServiceWithDepartmentsAsync(
        GetMedicalServiceInput input);
    
    Task<MedicalServiceWithDoctorsDto> GetMedicalServiceWithDoctorsAsync(
        GetMedicalServiceWithDoctorsInput input);
    
    Task<PagedResultDto<DoctorWithDetailsDto>> GetMedicalServiceDoctorsAsync(
        GetDepartmentServiceDoctorsInput input);
    
    Task<MedicalServiceDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<MedicalServiceDto> CreateAsync(MedicalServiceCreateDto input);

    Task<MedicalServiceDto> UpdateAsync(Guid id, MedicalServiceUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalServiceExcelDownloadDto input);

    Task DeleteByIdsAsync(List<Guid> medicalServiceIds);

    Task DeleteAllAsync(GetMedicalServiceInput input);
    Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}