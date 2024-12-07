using System;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare;

public interface ILookupAppService : IApplicationService
{
    Task<PagedResultDto<LookupDto<Guid>>> GetAppointmentTypeLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<LookupDto<Guid>>> GetMedicalServiceLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input);
    
    Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input);
}