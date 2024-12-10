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
public class DepartmentController(IDepartmentsAppService departmentsAppService) : HealthCareController, IDepartmentsAppService
{
    [HttpGet]
    public virtual Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
    {
        return departmentsAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<DepartmentDto> GetAsync(Guid id)
    {
        return departmentsAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<DepartmentDto> CreateAsync(DepartmentCreateDto input)
    {
        return departmentsAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<DepartmentDto> UpdateAsync(Guid id, DepartmentUpdateDto input)
    {
        return departmentsAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return departmentsAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("as-excel-file")]
    public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(DepartmentExcelDownloadDto input)
    {
        return departmentsAppService.GetListAsExcelFileAsync(input);
    }

    [HttpGet]
    [Route("download-token")]
    public virtual Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        return departmentsAppService.GetDownloadTokenAsync();
    }

    [HttpDelete]
    [Route("")]
    public virtual Task DeleteByIdsAsync(List<Guid> departmentIds)
    {
        return departmentsAppService.DeleteByIdsAsync(departmentIds);
    }

    [HttpDelete]
    [Route("all")]
    public virtual Task DeleteAllAsync(GetDepartmentsInput input)
    {
        return departmentsAppService.DeleteAllAsync(input);
    }
}