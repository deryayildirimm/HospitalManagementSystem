using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Departments;

public interface IDepartmentsAppService : IApplicationService
{
    Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input);

    Task<DepartmentDto> GetAsync(Guid id);

    Task DeleteAsync(Guid id);

    Task<DepartmentDto> CreateAsync(DepartmentCreateDto input);

    Task<DepartmentDto> UpdateAsync(Guid id, DepartmentUpdateDto input);

    Task<IRemoteStreamContent> GetListAsExcelFileAsync(DepartmentExcelDownloadDto input);
    Task DeleteByIdsAsync(List<Guid> departmentIds);

    Task DeleteAllAsync(GetDepartmentsInput input);
    Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();

}