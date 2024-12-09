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
    public Task<PagedResultDto<DoctorWorkingHoursDto>> GetListAsync(GetDoctorWorkingHoursInput input)
        => doctorWorkingHourAppService.GetListAsync(input);
}