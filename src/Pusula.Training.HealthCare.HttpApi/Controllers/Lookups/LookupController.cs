using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Controllers.Lookups;

[RemoteService]
[Area("app")]
[ControllerName("Lookup")]
[Route("api/app/lookups")]
public class LookupController(ILookupAppService lookupAppService) : HealthCareController, ILookupAppService
{
    [HttpGet]
    [Route("appointment-types")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetAppointmentTypeLookupAsync(LookupRequestDto input)
        => lookupAppService.GetAppointmentTypeLookupAsync(input);

    [HttpGet]
    [Route("medical-services")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetMedicalServiceLookupAsync(LookupRequestDto input)
        => lookupAppService.GetMedicalServiceLookupAsync(input);

    [HttpGet]
    [Route("departments")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
        => lookupAppService.GetDepartmentLookupAsync(input);

    [HttpGet]
    [Route("titles")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input)
        => lookupAppService.GetTitleLookupAsync(input);

    [HttpGet]
    [Route("cities")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
        => lookupAppService.GetCityLookupAsync(input);

    [HttpGet]
    [Route("districts")]
    public Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input)
        => lookupAppService.GetDistrictLookupAsync(input);
}