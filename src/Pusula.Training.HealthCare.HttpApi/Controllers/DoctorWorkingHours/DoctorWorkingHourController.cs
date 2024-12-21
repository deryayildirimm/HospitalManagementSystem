using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.DoctorWorkingHours;

[RemoteService]
[Area("app")]
[ControllerName("DoctorWorkingHour")]
[Route("api/app/doctor-working-hours")]
public class DoctorWorkingHourController(IDoctorWorkingHourAppService doctorWorkingHourAppService)
    : HealthCareController, IDoctorWorkingHourAppService
{
    [HttpGet]
    public virtual Task<PagedResultDto<DoctorWorkingHoursDto>> GetListAsync(GetDoctorWorkingHoursInput input)
        => doctorWorkingHourAppService.GetListAsync(input);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<DoctorWorkingHoursDto> GetAsync(Guid id)
        => doctorWorkingHourAppService.GetAsync(id);

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> ids)
        => doctorWorkingHourAppService.DeleteByIdsAsync(ids);

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
        => doctorWorkingHourAppService.DeleteAsync(id);
}