using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Departments;

[RemoteService]
[Area("app")]
[ControllerName("Department")]
[Route("api/app/departments")]
public class DepartmentController : HealthCareController, IDepartmentsAppService
{
    protected IDepartmentsAppService _departmentsAppService;

    public DepartmentController(IDepartmentsAppService departmentsAppService)
    {
        _departmentsAppService = departmentsAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
    {
        return _departmentsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<DepartmentDto> GetAsync(Guid id)
    {
        return _departmentsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<DepartmentDto> CreateAsync(DepartmentCreateDto input)
    {
        return _departmentsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<DepartmentDto> UpdateAsync(Guid id, DepartmentUpdateDto input)
    {
        return _departmentsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _departmentsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(DepartmentExcelDownloadDto input)
    {
        return _departmentsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return _departmentsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> departmentIds)
    {
        return _departmentsAppService.DeleteByIdsAsync(departmentIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetDepartmentsInput input)
    {
        return _departmentsAppService.DeleteAllAsync(input);
    }
}