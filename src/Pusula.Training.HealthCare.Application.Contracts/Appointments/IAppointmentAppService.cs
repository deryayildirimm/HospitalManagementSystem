using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Appointments;

public interface IAppointmentAppService : IApplicationService
{
    Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input);

    Task<AppointmentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<AppointmentDto> CreateAsync(AppointmentCreateDto input);

    Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> appointmentIds);

    Task DeleteAllAsync(GetAppointmentsInput input);
    Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
}