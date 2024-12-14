using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Restrictions;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.MedicalServices.Default)]
public class RestrictionAppService(
    IRestrictionRepository restrictionRepository,
    IRestrictionManager restrictionManager,
    IDistributedCache<RestrictionDownloadTokenCacheItem, string> downloadTokenCache
) : HealthCareAppService, IRestrictionAppService
{
    public virtual async Task<PagedResultDto<RestrictionDto>> GetListAsync(GetRestrictionsInput input)
    {
        var totalCount = await restrictionRepository.GetCountAsync(
            doctorId: input.DoctorId,
            medicalServiceId: input.MedicalServiceId,
            departmentId: input.DepartmentId);

        var items = await restrictionRepository.GetListAsync(
            medicalServiceId: input.MedicalServiceId,
            departmentId: input.DepartmentId,
            doctorId: input.DoctorId,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<RestrictionDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Restriction>, List<RestrictionDto>>(items)
        };
    }

    public virtual async Task<RestrictionDto> GetAsync(Guid id)
        => ObjectMapper.Map<Restriction, RestrictionDto>(await restrictionRepository.GetAsync(id));

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(RestrictionExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }
        
        var items = await restrictionRepository.GetListAsync(
            medicalServiceId: input.MedicalServiceId,
            departmentId: input.DepartmentId,
            doctorId: input.DoctorId);
        
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(
            ObjectMapper.Map<List<Restriction>, List<RestrictionExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Appointments.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

    }

    public virtual async Task DeleteAsync(Guid id)
        => await restrictionRepository.DeleteAsync(id);

    [Authorize(HealthCarePermissions.MedicalServices.Create)]
    public virtual async Task<RestrictionDto> CreateAsync(RestrictionCreateDto input)
    {
        
        var restriction = await restrictionManager.CreateAsync(
            input.MedicalServiceId,
            input.DepartmentId,
            input.DoctorId,
            input.MinAge,
            input.MaxAge,
            input.AllowedGender
        );

        return ObjectMapper.Map<Restriction, RestrictionDto>(restriction);
    }

    [Authorize(HealthCarePermissions.MedicalServices.Edit)]
    public virtual async Task<RestrictionDto> UpdateAsync(Guid id, RestrictionUpdateDto input)
    {
        var restriction = await restrictionManager.UpdateAsync(
            id,
            input.MedicalServiceId,
            input.DepartmentId,
            input.DoctorId,
            input.MinAge,
            input.MaxAge,
            input.AllowedGender
        );

        return ObjectMapper.Map<Restriction, RestrictionDto>(restriction);
    }

    [Authorize(HealthCarePermissions.MedicalServices.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> ids)
        => await restrictionRepository.DeleteManyAsync(ids);

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new RestrictionDownloadTokenCacheItem { Token = token },
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