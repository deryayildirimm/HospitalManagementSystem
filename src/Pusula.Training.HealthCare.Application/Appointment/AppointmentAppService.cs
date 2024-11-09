using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Appointment;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Appointments.Default)]
public class AppointmentAppService(
    IDistributedCache<MedicalServiceDownloadTokenCacheItem, string> downloadCache
) : HealthCareAppService, IAppointmentAppService
{
    public Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    public Task<DepartmentDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<DepartmentDto> CreateAsync(AppointmentCreateDto input)
    {
        throw new NotImplementedException();
    }

    public Task<DepartmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        throw new NotImplementedException();
    }

    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdsAsync(List<Guid> appointmentIds)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        throw new NotImplementedException();
    }
}