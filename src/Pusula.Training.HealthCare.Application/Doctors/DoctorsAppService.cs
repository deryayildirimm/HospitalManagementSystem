using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Doctors;


[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Doctors.Default)]
public class DoctorsAppService(
        IDoctorRepository doctorRepository,
        DoctorManager doctorManager,
        IDistributedCache<DoctorDownloadTokenCacheItem, string> downloadTokenCache,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        ITitleRepository titleRepository,
        IDepartmentRepository departmentRepository) : HealthCareAppService, IDoctorsAppService
{
    public virtual async Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetListAsync(GetDoctorsInput input)
    {
        var totalCount = await doctorRepository.GetCountAsync(input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.TitleId, input.DepartmentId);
        var items = await doctorRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.TitleId, input.DepartmentId, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<DoctorWithNavigationPropertiesDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<DoctorWithNavigationProperties>, List<DoctorWithNavigationPropertiesDto>>(items)
        };
    }

    public virtual async Task<DoctorWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return ObjectMapper.Map<DoctorWithNavigationProperties, DoctorWithNavigationPropertiesDto>
            (await doctorRepository.GetWithNavigationPropertiesAsync(id));
    }

    public virtual async Task<DoctorDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<Doctor, DoctorDto>(await doctorRepository.GetAsync(id));
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetCityLookupAsync(LookupRequestDto input)
    {
        var query = (await cityRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name != null && x.Name.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<City>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<City>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(LookupRequestDto input)
    {
        var query = (await districtRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name != null && x.Name.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<District>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<District>, List<LookupDto<Guid>>>(lookupData)
        };
    }



    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetTitleLookupAsync(LookupRequestDto input)
    {
        var query = (await titleRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.TitleName != null && x.TitleName.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Title>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Title>, List<LookupDto<Guid>>>(lookupData)
        };
    }

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
    {
        var query = (await departmentRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                x => x.Name != null && x.Name.Contains(input.Filter!));

        var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Department>();
        var totalCount = query.Count();
        return new PagedResultDto<LookupDto<Guid>>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Department>, List<LookupDto<Guid>>>(lookupData)
        };
    }
    
    public virtual async Task<PagedResultDto<DoctorWithNavigationPropertiesDto>> GetByDepartmentIdsAsync(GetDoctorsWithDepartmentIdsInput input)
    {
        var totalCount = await doctorRepository.GetCountByDepartmentIdsAsync(input.FilterText, input.DepartmentIds!);
        var items = await doctorRepository.GetListByDepartmentIdsAsync(input.FilterText, input.DepartmentIds!, 
            input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<DoctorWithNavigationPropertiesDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<DoctorWithNavigationProperties>, List<DoctorWithNavigationPropertiesDto>>(items)
        };
    }


    [Authorize(HealthCarePermissions.Doctors.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await doctorRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.Doctors.Create)]
    public virtual async Task<DoctorDto> CreateAsync(DoctorCreateDto input)
    {
        if (input.CityId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
        }
        if (input.DistrictId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["District"]]);
        }
        if (input.TitleId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Title"]]);
        }
        if (input.DepartmentId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
        }

        var doctor = await doctorManager.CreateAsync(
            input.CityId, input.DistrictId,input.TitleId, input.DepartmentId, input.FirstName, input.LastName, 
            input.IdentityNumber, input.BirthDate, input.Gender, input.StartDate, input.Email, input.PhoneNumber
        );

        return ObjectMapper.Map<Doctor, DoctorDto>(doctor);
    }

    [Authorize(HealthCarePermissions.Doctors.Edit)]
    public virtual async Task<DoctorDto> UpdateAsync(DoctorUpdateDto input)
    {
        if (input.TitleId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Title"]]);
        }
        if (input.DepartmentId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
        }

        var doctor = await doctorManager.UpdateAsync(
        input.Id, input.CityId, input.DistrictId,input.TitleId, input.DepartmentId, input.FirstName, input.LastName, 
        input.IdentityNumber, input.BirthDate, input.Gender, input.StartDate, input.Email, input.PhoneNumber);

        return ObjectMapper.Map<Doctor, DoctorDto>(doctor);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(DoctorExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var doctors = await doctorRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.TitleId, input.DepartmentId);
        var items = doctors.Select(item => new
        {
            item.Doctor.FirstName,
            item.Doctor.LastName,
            item.Doctor.IdentityNumber,

            City = item.City?.Name,
            District = item.District?.Name,
            Title = item.Title?.TitleName,
            Department = item.Department?.Name,

        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Doctors.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.Doctors.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> doctorIds)
    {
        await doctorRepository.DeleteManyAsync(doctorIds);
    }

    [Authorize(HealthCarePermissions.Doctors.Delete)]
    public virtual async Task DeleteAllAsync(GetDoctorsInput input)
    {
        await doctorRepository.DeleteAllAsync( input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.TitleId, input.DepartmentId);
    }
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new DoctorDownloadTokenCacheItem { Token = token },
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