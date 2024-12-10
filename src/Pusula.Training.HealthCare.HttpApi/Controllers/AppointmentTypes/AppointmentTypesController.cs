using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.AppointmentTypes;

[RemoteService]
[Area("app")]
[ControllerName("AppointmentType")]
[Route("api/app/appointment-types")]
public class AppointmentTypesController(IAppointmentTypesAppService appointmentTypesAppService)
    : HealthCareController, IAppointmentTypesAppService
{
    [HttpGet]
    public virtual Task<PagedResultDto<AppointmentTypeDto>> GetListAsync(GetAppointmentTypesInput input)
        => appointmentTypesAppService.GetListAsync(input);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<AppointmentTypeDto> GetAsync(Guid id)
        => appointmentTypesAppService.GetAsync(id);

    [HttpPost]
    public virtual Task<AppointmentTypeDto> CreateAsync(AppointmentTypeCreateDto input)
        => appointmentTypesAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public virtual Task<AppointmentTypeDto> UpdateAsync(Guid id, AppointmentTypeUpdateDto input)
        => appointmentTypesAppService.UpdateAsync(id, input);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
        => appointmentTypesAppService.DeleteAsync(id);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> appointmentTypeIds)
        => appointmentTypesAppService.DeleteByIdsAsync(appointmentTypeIds);

    [HttpGet]
    [Route("as-excel-file")]
    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentTypeExcelDownloadDto input)
        => appointmentTypesAppService.GetListAsExcelFileAsync(input);

    [HttpGet]
    [Route("download-token")]
    public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        => appointmentTypesAppService.GetDownloadTokenAsync();
}