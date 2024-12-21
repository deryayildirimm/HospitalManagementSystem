using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.GlobalExceptions;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.MedicalServices;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.MedicalServices.Default)]
public class MedicalServicesAppService(
    IMedicalServiceRepository medicalServiceRepository,
    MedicalServiceManager medicalServiceManager,
    IDistributedCache<MedicalServiceDownloadTokenCacheItem, string> downloadCache
) : HealthCareAppService, IMedicalServicesAppService
{
    public virtual async Task<PagedResultDto<MedicalServiceDto>> GetListAsync(GetMedicalServiceInput input)
    {
        var totalCount =
            await medicalServiceRepository.GetCountAsync(
                input.DepartmentId, input.Name, input.CostMin, input.CostMax, input.ServiceDateMin,
                input.ServiceDateMax);

        var items = await medicalServiceRepository.GetListAsync(
            input.DepartmentId,
            input.Name,
            input.CostMin,
            input.CostMax,
            input.ServiceDateMin,
            input.ServiceDateMax,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<MedicalServiceDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MedicalService>, List<MedicalServiceDto>>(items)
        };
    }

    public virtual async Task<PagedResultDto<MedicalServiceDto>> GetMedicalServiceByDepartmentIdAsync(
        GetServiceByDepartmentInput input)
    {
        var items = await medicalServiceRepository.GetMedicalServiceListByDepartmentIdAsync(input.DepartmentId,
            input.Sorting,
            input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<MedicalServiceDto>
        {
            TotalCount = items.Count,
            Items = ObjectMapper.Map<List<MedicalService>, List<MedicalServiceDto>>(items)
        };
    }

    public async Task<PagedResultDto<MedicalServiceWithDepartmentsDto>> GetMedicalServiceWithDepartmentsAsync(
        GetMedicalServiceInput input)
    {
        var totalCount =
            await medicalServiceRepository.GetCountAsync(
                input.DepartmentId, input.Name, input.CostMin, input.CostMax, input.ServiceDateMin,
                input.ServiceDateMax);

        var items = await medicalServiceRepository.GetMedicalServiceWithDepartmentsAsync(input.Name,
            input.CostMin, input.CostMax, input.ServiceDateMin, input.ServiceDateMax, input.Sorting,
            input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<MedicalServiceWithDepartmentsDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MedicalServiceWithDepartments>, List<MedicalServiceWithDepartmentsDto>>(items)
        };
    }

    public virtual async Task<MedicalServiceWithDoctorsDto> GetMedicalServiceWithDoctorsAsync(
        GetMedicalServiceInput input)
    {
        var result = await medicalServiceRepository.GetMedicalServiceWithDoctorsAsync(input.MedicalServiceId,
            input.DepartmentId,
            input.Name,
            input.CostMin, input.CostMax, input.ServiceDateMin, input.ServiceDateMax, input.Sorting,
            input.MaxResultCount, input.SkipCount);

        return ObjectMapper.Map<MedicalServiceWithDoctors, MedicalServiceWithDoctorsDto>(result);
    }

    public virtual async Task<PagedResultDto<DoctorWithDetailsDto>> GetMedicalServiceDoctorsAsync(
        GetMedicalServiceInput input)
    {
        var result = await medicalServiceRepository.GetMedicalServiceDoctorsAsync(
            input.MedicalServiceId,
            input.DepartmentId,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<DoctorWithDetailsDto>
        {
            TotalCount = result.Count,
            Items = ObjectMapper.Map<List<DoctorWithDetails>, List<DoctorWithDetailsDto>>(result)
        };
    }


    public virtual async Task<MedicalServiceDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<MedicalService, MedicalServiceDto>(await medicalServiceRepository.GetAsync(id));
    }

    [Authorize(HealthCarePermissions.MedicalServices.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await medicalServiceRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.MedicalServices.Create)]
    public virtual async Task<MedicalServiceDto> CreateAsync(MedicalServiceCreateDto input)
    {
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.NameAlreadyExists,
            await medicalServiceRepository.FirstOrDefaultAsync(x => x.Name == input.Name) is not null);

        var medicalService = await medicalServiceManager.CreateAsync(
            input.Name,
            input.ServiceCreatedAt,
            input.Cost,
            input.Duration,
            input.DepartmentNames
        );

        return ObjectMapper.Map<MedicalService, MedicalServiceDto>(medicalService);
    }


    [Authorize(HealthCarePermissions.MedicalServices.Edit)]
    public virtual async Task<MedicalServiceDto> UpdateAsync(Guid id, MedicalServiceUpdateDto input)
    {
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorNotWorking,
            await medicalServiceRepository.FirstOrDefaultAsync(x => x.Name == input.Name && x.Id != id) is not null);

        var medicalService = await medicalServiceManager.UpdateAsync(
            id,
            input.Name,
            input.Cost,
            input.Duration,
            input.ServiceCreatedAt,
            input.DepartmentNames,
            input.ConcurrencyStamp
        );

        return ObjectMapper.Map<MedicalService, MedicalServiceDto>(medicalService);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalServiceExcelDownloadDto input)
    {
        var downloadToken = await downloadCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await medicalServiceRepository.GetListAsync(input.DepartmentId,
            input.MedicalServiceName,
            input.CostMin, input.CostMax, input.ServiceDateMin, input.ServiceDateMax);

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(ObjectMapper.Map<List<MedicalService>, List<MedicalServiceExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "MedicalServices.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task DeleteByIdsAsync(List<Guid> medicalServiceIds)
        => await medicalServiceRepository.DeleteManyAsync(medicalServiceIds);


    [Authorize(HealthCarePermissions.MedicalServices.Delete)]
    public virtual async Task DeleteAllAsync(GetMedicalServiceInput input)
        => await medicalServiceRepository.DeleteAllAsync(input.Name, input.CostMin, input.CostMax);

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadCache.SetAsync(
            token,
            new MedicalServiceDownloadTokenCacheItem { Token = token },
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