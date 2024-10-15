using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Departments
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Departments.Default)]
    public class DepartmentsAppService(IDepartmentRepository departmentRepository, 
        DepartmentManager departmentManager, IDistributedCache<DepartmentDownloadTokenCacheItem, string> downloadTokenCache) 
        : HealthCareAppService, IDepartmentsAppService
    {
        public virtual async Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
        {
            var totalCount = await departmentRepository.GetCountAsync(input.FilterText, input.Name);
            var items = await departmentRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<DepartmentDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(items)
            };
        }

        public virtual async Task<DepartmentDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Department, DepartmentDto>(await departmentRepository.GetAsync(id));
        }

        [Authorize(HealthCarePermissions.Departments.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await departmentRepository.DeleteAsync(id);
        }

        [Authorize(HealthCarePermissions.Departments.Create)]
        public virtual async Task<DepartmentDto> CreateAsync(DepartmentCreateDto input)
        {

            var department = await departmentManager.CreateAsync(
            input.Name
            );

            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        [Authorize(HealthCarePermissions.Departments.Edit)]
        public virtual async Task<DepartmentDto> UpdateAsync(Guid id, DepartmentUpdateDto input)
        {

            var department = await departmentManager.UpdateAsync(
            id,
            input.Name, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Department, DepartmentDto>(department);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DepartmentExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await departmentRepository.GetListAsync(input.FilterText, input.Name);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Department>, List<DepartmentExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Departments.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Departments.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> departmentIds)
        {
            await departmentRepository.DeleteManyAsync(departmentIds);
        }

        [Authorize(HealthCarePermissions.Departments.Delete)]
        public virtual async Task DeleteAllAsync(GetDepartmentsInput input)
        {
            await departmentRepository.DeleteAllAsync(input.FilterText, input.Name);
        }
        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new DepartmentDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}