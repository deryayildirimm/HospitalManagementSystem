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
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.MedicalPersonnel;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.MedicalStaff.Default)]
public class MedicalStaffAppService(
        IMedicalStaffRepository medicalStaffRepository,
        MedicalStaffManager medicalStaffManager,
        IDistributedCache<MedicalStaffDownloadTokenCacheItem, string> downloadTokenCache,
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        IDepartmentRepository departmentRepository) : HealthCareAppService, IMedicalStaffAppService
{
    public virtual async Task<PagedResultDto<MedicalStaffWithNavigationPropertiesDto>> GetListAsync(GetMedicalStaffInput input)
    {
        var totalCount = await medicalStaffRepository.GetCountAsync(input.FilterText, input.FirstName, input.LastName, 
            input.IdentityNumber, input.BirthDateMin, input.BirthDateMax, input.Gender, input.Email, input.PhoneNumber, 
            input.YearOfExperienceMin, input.CityId, input.DistrictId, input.DepartmentId);
        var items = await medicalStaffRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, 
            input.DepartmentId, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<MedicalStaffWithNavigationPropertiesDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<MedicalStaffWithNavigationProperties>, List<MedicalStaffWithNavigationPropertiesDto>>(items)
        };
    }

    public virtual async Task<MedicalStaffWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
    {
        return ObjectMapper.Map<MedicalStaffWithNavigationProperties, MedicalStaffWithNavigationPropertiesDto>
            (await medicalStaffRepository.GetWithNavigationPropertiesAsync(id));
    }

    public virtual async Task<MedicalStaffDto> GetAsync(Guid id)
    {
        return ObjectMapper.Map<MedicalStaff, MedicalStaffDto>(await medicalStaffRepository.GetAsync(id));
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

    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDistrictLookupAsync(Guid? cityId, LookupRequestDto input)
    {
        var query = (await districtRepository.GetQueryableAsync())
            .WhereIf(cityId.HasValue, x => x.CityId == cityId!.Value) 
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

    [Authorize(HealthCarePermissions.MedicalStaff.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await medicalStaffRepository.DeleteAsync(id);
    }

    [Authorize(HealthCarePermissions.MedicalStaff.Create)]
    public virtual async Task<MedicalStaffDto> CreateAsync(MedicalStaffCreateDto input)
    {
        if (input.CityId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
        }
        if (input.DistrictId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["District"]]);
        }
        if (input.DepartmentId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
        }

        var medicalStaff = await medicalStaffManager.CreateAsync(
            input.CityId, input.DistrictId, input.DepartmentId, input.FirstName, input.LastName, 
            input.IdentityNumber, input.BirthDate, input.Gender, input.StartDate, input.Email, input.PhoneNumber
        );

        return ObjectMapper.Map<MedicalStaff, MedicalStaffDto>(medicalStaff);
    }

    [Authorize(HealthCarePermissions.MedicalStaff.Edit)]
    public virtual async Task<MedicalStaffDto> UpdateAsync(MedicalStaffUpdateDto input)
    {
        if (input.CityId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["City"]]);
        }
        if (input.DistrictId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["District"]]);
        }
        if (input.DepartmentId == default)
        {
            throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
        }

        var medicalStaff = await medicalStaffManager.UpdateAsync(
        input.Id, input.CityId, input.DistrictId, input.DepartmentId, input.FirstName, input.LastName, 
        input.IdentityNumber, input.BirthDate, input.Gender, input.StartDate, input.Email, input.PhoneNumber);

        return ObjectMapper.Map<MedicalStaff, MedicalStaffDto>(medicalStaff);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MedicalStaffExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var medicalStaffs = await medicalStaffRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.DepartmentId);
        var items = medicalStaffs.Select(item => new
        {
            item.MedicalStaff.FirstName,
            item.MedicalStaff.LastName,
            item.MedicalStaff.IdentityNumber,

            City = item.City?.Name,
            District = item.District?.Name,
            Department = item.Department?.Name,

        });

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(items);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "MedicalStaff.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.MedicalStaff.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> medicalStaffIds)
    {
        await medicalStaffRepository.DeleteManyAsync(medicalStaffIds);
    }

    [Authorize(HealthCarePermissions.MedicalStaff.Delete)]
    public virtual async Task DeleteAllAsync(GetMedicalStaffInput input)
    {
        await medicalStaffRepository.DeleteAllAsync( input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax,
            input.Gender, input.Email, input.PhoneNumber, input.YearOfExperienceMin, input.CityId, input.DistrictId, input.DepartmentId);
    }
    
    public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new MedicalStaffDownloadTokenCacheItem { Token = token },
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