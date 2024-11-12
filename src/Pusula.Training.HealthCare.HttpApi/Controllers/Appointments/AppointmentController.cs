using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Appointments;
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
    [HttpGet]
    public Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("{id}")]
    public Task<AppointmentDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    public Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    [Route("{id}")]
    public Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("")]
    public Task DeleteByIdsAsync(List<Guid> appointmentIds)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    [Route("all")]
    public Task DeleteAllAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        throw new NotImplementedException();
    }
}