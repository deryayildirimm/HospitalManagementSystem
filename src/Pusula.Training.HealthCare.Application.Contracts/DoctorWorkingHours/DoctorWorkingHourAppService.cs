using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Doctors;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public interface IDoctorWorkingHourAppService : IApplicationService
{
    Task<PagedResultDto<DoctorWorkingHoursDto>> GetListAsync(GetDoctorWorkingHoursInput input);
    
    Task<DoctorWorkingHoursDto> GetAsync(Guid id);
    
    Task DeleteByIdsAsync(List<Guid> ids);
    
    Task DeleteAsync(Guid id);
    
}