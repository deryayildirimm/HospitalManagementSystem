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
public class AppointmentController(IAppointmentAppService appointmentAppService)
    : HealthCareController, IAppointmentAppService
{
    [HttpGet]
    [Route("available-days-lookup")]
    public virtual Task<PagedResultDto<AppointmentDayLookupDto>> GetAvailableDaysLookupAsync(
        GetAppointmentsLookupInput input)
        => appointmentAppService.GetAvailableDaysLookupAsync(input);

    [HttpGet]
    [Route("available-slots")]
    public async Task<PagedResultDto<AppointmentSlotDto>> GetAvailableSlotsAsync(GetAppointmentSlotInput input) =>
        await appointmentAppService.GetAvailableSlotsAsync(input);

    [HttpGet]
    public virtual Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
        => appointmentAppService.GetListAsync(input);

    [HttpGet]
    [Route("statistics")]
    public virtual Task<PagedResultDto<GroupedAppointmentCountDto>> GetCountByGroupAsync(GetAppointmentsInput input)
      => appointmentAppService.GetCountByGroupAsync(input);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<AppointmentDto> GetAsync(Guid id)
        => appointmentAppService.GetAsync(id);

    [HttpPost]
    public virtual Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
        => appointmentAppService.CreateAsync(input);

    [HttpPut]
    [Route("{id}")]
    public virtual Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
        => appointmentAppService.UpdateAsync(id, input);

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
        => appointmentAppService.GetListAsExcelFileAsync(input);


    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
        => appointmentAppService.DeleteAsync(id);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> appointmentIds)
        => appointmentAppService.DeleteByIdsAsync(appointmentIds);

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetAppointmentsInput input)
        => appointmentAppService.DeleteAllAsync(input);

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        => appointmentAppService.GetDownloadTokenAsync();
}