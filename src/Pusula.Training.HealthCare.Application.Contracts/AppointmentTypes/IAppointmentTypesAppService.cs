using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.AppointmentTypes;

public interface IAppointmentTypesAppService : IApplicationService
{
    Task<PagedResultDto<AppointmentTypeDto>> GetListAsync(GetAppointmentTypesInput input);

    Task<AppointmentTypeDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<AppointmentTypeDto> CreateAsync(AppointmentTypeCreateDto input);

    Task<AppointmentTypeDto> UpdateAsync(Guid id, AppointmentTypeUpdateDto input);
    
    Task DeleteByIdsAsync(List<Guid> appointmentTypeIds);
    
    Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentTypeExcelDownloadDto input);
    
    Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

}