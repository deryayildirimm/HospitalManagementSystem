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
using AutoMapper.Configuration.Annotations;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Insurances;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
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
        IRepository<Departments.Department, Guid> departmentRepository,
        IRepository<Doctors.Doctor, Guid> doctorRepository,
        IRepository<ProtocolTypes.ProtocolType, Guid> protocolTypeRepository,
        IRepository<Insurances.Insurance, Guid> insuranceRepository) : HealthCareAppService, IProtocolsAppService
    {
        public virtual async Task<PagedResultDto<ProtocolWithNavigationPropertiesDto>> GetListAsync(GetProtocolsInput input)
        {
            var totalCount = await protocolRepository.GetCountAsync(input.FilterText, input.Notes, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin,input.EndTimeMax, input.PatientId, input.DepartmentId, input.ProtocolTypeId, input.DoctorId, input.InsuranceId);
            var items = await protocolRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Notes, input.StartTimeMin, input.StartTimeMax,  input.EndTimeMin,input.EndTimeMax,input.PatientId, input.DepartmentId, input.ProtocolTypeId, input.DoctorId, input.InsuranceId,input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProtocolWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProtocolWithNavigationProperties>, List<ProtocolWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ProtocolDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<Protocol, ProtocolDto>
                (await protocolRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ProtocolDto> GetAsync(Guid id )
        {

            var protocol = await protocolRepository.GetAsync(id);
            
            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }


        public virtual async Task<PagedResultDto<DepartmentStatisticDto>> GetDepartmentPatientStatisticsAsync(
            GetProtocolsInput input)
        {

            var count = await protocolRepository.GetGroupCountByDepartmentPatientAsync(
                departmentName: input.DepartmentName,
                patientId: input.PatientId,
                departmentId: input.DepartmentId,
                protocolTypeId: input.ProtocolTypeId,
                doctorId: input.DoctorId,
                insuranceId: input.InsuranceId,
                startTimeMin: input.StartTimeMin,
                startTimeMax: input.StartTimeMax,
                endTimeMin: input.EndTimeMin,
                endTimeMax: input.EndTimeMax);

            var items = await protocolRepository.GetGroupByDepartmentPatientAsync(
                departmentName: input.DepartmentName,
                patientId: input.PatientId,
                departmentId: input.DepartmentId,
                protocolTypeId: input.ProtocolTypeId,
                doctorId: input.DoctorId,
                insuranceId: input.InsuranceId,
                startTimeMin: input.StartTimeMin,
                startTimeMax: input.StartTimeMax,
                endTimeMin: input.EndTimeMin,
                endTimeMax: input.EndTimeMax
            );
            
            return new PagedResultDto<DepartmentStatisticDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<DepartmentStatistic>, List<DepartmentStatisticDto>>(items)
            };



        }
        
        

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input)
        {
            var query = (await patientRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FirstName.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Patients.Patient>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Patients.Patient>, List<LookupDto<Guid>>>(lookupData)
            };
        }


        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProtocolTypeLookUpAsync(LookupRequestDto input)
        {
            var query = (await protocolTypeRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name.Contains(input.Filter));
            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<ProtocolTypes.ProtocolType>();
            var totalCount = query.Count();

            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProtocolTypes.ProtocolType>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDoctorLookUpAsync(LookupRequestDto input)
        {
            var query = (await doctorRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FirstName != null && x.FirstName.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Doctors.Doctor>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Doctors.Doctor>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetInsuranceLookUpAsync(LookupRequestDto input)
        {
            var query = (await insuranceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.InsuranceCompanyName.ToString() != null && x.InsuranceCompanyName.ToString().Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Insurances.Insurance>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Insurances.Insurance>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
        {
            var query = (await departmentRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Departments.Department>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Departments.Department>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteAsync(Guid id) =>  await protocolRepository.DeleteAsync(id);
        

        [Authorize(HealthCarePermissions.Protocols.Create)]
        public virtual async Task<ProtocolDto> CreateAsync(ProtocolCreateDto input)
        {
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Patient"]],
                input.PatientId ==  Guid.Empty
            );

            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Department"]],
                input.DepartmentId ==  Guid.Empty
            );
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Doctor"]],
                input.DoctorId == Guid.Empty
            );
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["ProtocolType"]],
                input.ProtocolTypeId ==  Guid.Empty
            );
            
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Insurance"]],
                input.InsuranceId ==  Guid.Empty
            );
            var protocol = await protocolManager.CreateAsync(
            input.PatientId, input.DepartmentId, input.ProtocolTypeId, input.DoctorId, input.InsuranceId, input.StartTime, input.Notes, input.EndTime
            );

            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }

        [Authorize(HealthCarePermissions.Protocols.Edit)]
        public virtual async Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input)
        {
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Patient"]],
                input.PatientId ==  Guid.Empty
            );

            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Department"]],
                input.DepartmentId ==  Guid.Empty
            );
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Doctor"]],
                input.DoctorId == Guid.Empty
            );
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["ProtocolType"]],
                input.ProtocolTypeId ==  Guid.Empty
            );
            HealthCareGlobalException.ThrowIf(
                L["The {0} field is required.", L["Insurance"]],
                input.InsuranceId ==  Guid.Empty
            );
            var protocol = await protocolManager.UpdateAsync(
            id,
            input.PatientId, input.DepartmentId, input.ProtocolTypeId,  input.DoctorId,  input.InsuranceId, input.StartTime, input.Notes, input.EndTime, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }

       
        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProtocolExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorCodes.InvalidDownloadToken_MESSAGE,
                HealthCareDomainErrorCodes.InvalidDownloadToken_CODE,
                (downloadToken == null || input.DownloadToken != downloadToken.Token));
            

            var protocols = await protocolRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Type, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin,input.EndTimeMax, input.PatientId, input.DepartmentId, input.ProtocolTypeId, input.DoctorId );
            var items = protocols.Select(item => new
            {

                Patient = item.Patient?.FirstName,
                Department = item.Department?.Name,
                Doctor = item.Doctor?.FirstName + " " + item.Doctor?.LastName,
                ProtocolType = item.ProtocolType?.Name,
                Insurance = item.Insurance?.InsuranceCompanyName
                

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Protocols.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> protocolIds)  =>   await protocolRepository.DeleteManyAsync(protocolIds);
     

        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteAllAsync(GetProtocolsInput input) => 
            await protocolRepository.DeleteAllAsync(input.FilterText, input.Notes, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin, input.EndTimeMax, input.PatientId, input.DepartmentId, input.DoctorId);
        
        
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