using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MiniExcelLibs;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;
using DistributedCacheEntryOptions = Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions;

namespace Pusula.Training.HealthCare.DoctorLeaves;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.DoctorLeaves.Default)]
public class DoctorLeaveAppService(
    IDoctorLeaveRepository doctorLeaveRepository,
    DoctorLeaveManager doctorLeaveManager,
    IDistributedCache<DoctorLeaveDownloadTokenCacheItem, string> downloadTokenCache,
    IDistributedEventBus distributedEventBus) : HealthCareAppService, IDoctorLeaveAppService
{
    public virtual async Task<PagedResultDto<DoctorLeaveDto>> GetListAsync(GetDoctorLeaveInput input)
    {
        var totalCount = await doctorLeaveRepository.GetCountAsync(input.FilterText, input.DoctorId, input.StartDateMin,
            input.StartDateMax, input.EndDateMin, input.EndDateMax, input.Reason);
        var items = await doctorLeaveRepository.GetListAsync(input.FilterText, input.DoctorId, input.StartDateMin,
            input.StartDateMax, input.EndDateMin, input.EndDateMax, input.Reason, input.Sorting, input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<DoctorLeaveDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<DoctorLeave>, List<DoctorLeaveDto>>(items)
        };
    }

    public virtual async Task<DoctorLeaveDto> GetAsync(Guid id)
    {
        await distributedEventBus.PublishAsync(new DoctorLeaveViewedEto() { Id = id, ViewedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        var leave = await doctorLeaveRepository.GetAsync(id);
        return ObjectMapper.Map<DoctorLeave, DoctorLeaveDto>(leave);
    }


    [Authorize(HealthCarePermissions.DoctorLeaves.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await doctorLeaveRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.DoctorLeaves.Create)]
    public virtual async Task<DoctorLeaveDto> CreateAsync(DoctorLeaveCreateDto input)
    {
        var leave = await doctorLeaveManager.CreateAsync(input.DoctorId, input.StartDate, input.EndDate,
            input.EnumLeaveType, input.Reason);
        return ObjectMapper.Map<DoctorLeave, DoctorLeaveDto>(leave);
    }

    [Authorize(HealthCarePermissions.DoctorLeaves.Edit)]
    public virtual async Task<DoctorLeaveDto> UpdateAsync(Guid id, DoctorLeaveUpdateDto input)
    {
        var leave = await doctorLeaveManager.UpdateAsync(id, input.DoctorId, input.StartDate, input.EndDate,
            input.EnumLeaveType, input.Reason, input.ConcurrencyStamp);

        return ObjectMapper.Map<DoctorLeave, DoctorLeaveDto>(leave);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorLeaveExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDownloadToken_MESSAGE,
            HealthCareDomainErrorCodes.InvalidDownloadToken_CODE,
            downloadToken == null || input.DownloadToken != downloadToken.Token);

        var items = await doctorLeaveRepository.GetListAsync(input.FilterText, input.DoctorId, input.StartDateMin,
            input.StartDateMax, input.EndDateMin, input.EndDateMax, input.Reason);

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<DoctorLeave>, List<DoctorLeaveExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Leaves.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.DoctorLeaves.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> leaveIds)
    {
        await doctorLeaveRepository.DeleteManyAsync(leaveIds);
    }

    [Authorize(HealthCarePermissions.DoctorLeaves.Delete)]
    public virtual async Task DeleteAllAsync(GetDoctorLeaveInput input)
    {
        await doctorLeaveRepository.DeleteAllAsync(input.FilterText, input.DoctorId, input.StartDateMin,
            input.StartDateMax, input.EndDateMin, input.EndDateMax,
            input.Reason);
    }

    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new DoctorLeaveDownloadTokenCacheItem { Token = token },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

        return new Shared.DownloadTokenResultDto
        {
            Token = token
        };
    }
}