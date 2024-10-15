using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Protocols
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Protocols.Default)]
    public class ProtocolsAppService(
        IProtocolRepository protocolRepository, 
        ProtocolManager protocolManager, 
        IDistributedCache<ProtocolDownloadTokenCacheItem, string> downloadTokenCache, 
        IRepository<Patients.Patient, Guid> patientRepository, 
        IRepository<Departments.Department, Guid> departmentRepository) : HealthCareAppService, IProtocolsAppService
    {
        public virtual async Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input)
        {
            var totalCount = await protocolRepository.GetCountAsync(input.FilterText, input.Type, input.StartTimeMin, input.StartTimeMax, input.EndTime, input.PatientId, input.DepartmentId);
            var items = await protocolRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Type, input.StartTimeMin, input.StartTimeMax, input.EndTime, input.PatientId, input.DepartmentId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProtocolWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProtocolWithNavigationProperties>, List<ProtocolWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ProtocolWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ProtocolWithNavigationProperties, ProtocolWithNavigationPropertiesDto>
                (await protocolRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ProtocolDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Protocol, ProtocolDto>(await protocolRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input)
        {
            var query = (await patientRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FirstName != null && x.FirstName.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Patients.Patient>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Patients.Patient>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
        {
            var query = (await departmentRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Departments.Department>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Departments.Department>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await protocolRepository.DeleteAsync(id);
        }

        [Authorize(HealthCarePermissions.Protocols.Create)]
        public virtual async Task<ProtocolDto> CreateAsync(ProtocolCreateDto input)
        {
            if (input.PatientId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Patient"]]);
            }
            if (input.DepartmentId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
            }

            var protocol = await protocolManager.CreateAsync(
            input.PatientId, input.DepartmentId, input.Type, input.StartTime, input.EndTime
            );

            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }

        [Authorize(HealthCarePermissions.Protocols.Edit)]
        public virtual async Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input)
        {
            if (input.PatientId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Patient"]]);
            }
            if (input.DepartmentId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Department"]]);
            }

            var protocol = await protocolManager.UpdateAsync(
            id,
            input.PatientId, input.DepartmentId, input.Type, input.StartTime, input.EndTime, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var protocols = await protocolRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Type, input.StartTimeMin, input.StartTimeMax, input.EndTime, input.PatientId, input.DepartmentId);
            var items = protocols.Select(item => new
            {
                item.Protocol.Type,
                item.Protocol.StartTime,
                item.Protocol.EndTime,

                Patient = item.Patient?.FirstName,
                Department = item.Department?.Name,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Protocols.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> protocolIds)
        {
            await protocolRepository.DeleteManyAsync(protocolIds);
        }

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteAllAsync(GetProtocolsInput input)
        {
            await protocolRepository.DeleteAllAsync(input.FilterText, input.Type, input.StartTimeMin, input.StartTimeMax, input.EndTime, input.PatientId, input.DepartmentId);
        }
        public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new ProtocolDownloadTokenCacheItem { Token = token },
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
}