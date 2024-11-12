using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Appointments;

[RemoteService]
[Area("app")]
[ControllerName("Appointment")]
[Route("api/app/appointment")]
public class AppointmentController : HealthCareController, IAppointmentAppService
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