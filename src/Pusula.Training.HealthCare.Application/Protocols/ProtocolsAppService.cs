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
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.ProtocolTypes;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Protocols
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Protocols.Default)]
    public class ProtocolsAppService(
        IProtocolRepository protocolRepository, 
        ProtocolManager protocolManager, 
        IDistributedEventBus distributedEventBus,
        IDistributedCache<ProtocolDownloadTokenCacheItem, string> downloadTokenCache, 
        IPatientRepository patientRepository,
        IMedicalServiceRepository medicalServiceRepository) : HealthCareAppService, IProtocolsAppService
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

        public virtual async Task<ProtocolDto> GetWithNavigationPropertiesAsync(Guid id) => ObjectMapper.Map<Protocol, ProtocolDto> (await protocolRepository.GetWithNavigationPropertiesAsync(id));
       

        public virtual async Task<ProtocolDto> GetAsync(Guid id )
        {
            
            await distributedEventBus.PublishAsync(new ProtocolsViewedEto { Id = id, ViewedAt = Clock.Now },
                onUnitOfWorkComplete: false);

            var protocol = await protocolRepository.GetAsync(id);
            
            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }
        
        public virtual async Task<ProtocolWithDetailsDto> GetProtocolDetailsAsync(Guid id )
        {
            var protocol = await protocolRepository.GetWithAsync(id);
            
            return ObjectMapper.Map<ProtocolWithDetails, ProtocolWithDetailsDto>(protocol);
        }
        
        public virtual async Task<PagedResultDto<ProtocolPatientDepartmentListReportDto>> GetPatientsByDepartmentAsync(
            GetProtocolsInput input)
        {

            var count = await protocolRepository.GetGPatientsCountByDepartmentAsync( input.DepartmentName, input.PatientId, input.DepartmentId, input.ProtocolTypeId,input.DoctorId, input.DepartmentId, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin, input.EndTimeMax);

            var items = await protocolRepository.GetGPatientsByDepartmentAsync(
                departmentName: input.DepartmentName,
                patientId: input.PatientId,
                departmentId: input.DepartmentId,
                protocolTypeId: input.ProtocolTypeId,
                doctorId: input.DoctorId,
                insuranceId: input.InsuranceId,
                startTimeMin: input.StartTimeMin,
                startTimeMax: input.StartTimeMax,
                endTimeMin: input.EndTimeMin,
                endTimeMax: input.EndTimeMax,
                sorting:input.Sorting,
                note: input.Notes,
                filterText: input.FilterText,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );
            
            return new PagedResultDto<ProtocolPatientDepartmentListReportDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<ProtocolPatientDepartmentListReport>, List<ProtocolPatientDepartmentListReportDto>>(items)
            };
            
        }

        public virtual async Task<PagedResultDto<ProtocolPatientDoctorListReportDto>> GetPatientsByDoctorAsync(
            GetProtocolsInput input)
        {

            var count = await protocolRepository.GetGPatientsCountByDoctorAsync( input.DepartmentName, input.PatientId, input.DepartmentId, input.ProtocolTypeId,input.DoctorId, input.DepartmentId, input.StartTimeMin, input.StartTimeMax, input.EndTimeMin, input.EndTimeMax);

            var items = await protocolRepository.GetGPatientsByDoctorAsync(
                departmentName: input.DepartmentName,
                patientId: input.PatientId,
                departmentId: input.DepartmentId,
                protocolTypeId: input.ProtocolTypeId,
                doctorId: input.DoctorId,
                insuranceId: input.InsuranceId,
                startTimeMin: input.StartTimeMin,
                startTimeMax: input.StartTimeMax,
                endTimeMin: input.EndTimeMin,
                endTimeMax: input.EndTimeMax,
                sorting:input.Sorting,
                note: input.Notes,
                filterText: input.FilterText,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );
            
            return new PagedResultDto<ProtocolPatientDoctorListReportDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<ProtocolPatientDoctorListReport>, List<ProtocolPatientDoctorListReportDto>>(items)
            };
            
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
                endTimeMax: input.EndTimeMax,
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );
            
            return new PagedResultDto<DepartmentStatisticDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<DepartmentStatistic>, List<DepartmentStatisticDto>>(items)
            };
            
        }
        
        public virtual async Task<PagedResultDto<DoctorStatisticDto>> GetDoctorPatientStatisticsAsync(
            GetProtocolsInput input)
        {

            var count = await protocolRepository.GetGroupCountByDoctorPatientAsync(
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

            var items = await protocolRepository.GetGroupByDoctorPatientAsync(
                departmentName: input.DepartmentName,
                patientId: input.PatientId,
                departmentId: input.DepartmentId,
                protocolTypeId: input.ProtocolTypeId,
                doctorId: input.DoctorId,
                insuranceId: input.InsuranceId,
                startTimeMin: input.StartTimeMin,
                startTimeMax: input.StartTimeMax,
                endTimeMin: input.EndTimeMin,
                endTimeMax: input.EndTimeMax,
                sorting: input.Sorting,
                maxResultCount: input.MaxResultCount,
                skipCount: input.SkipCount
            );
            
            return new PagedResultDto<DoctorStatisticDto>
            {
                TotalCount = count,
                Items = ObjectMapper.Map<List<DoctorStatistics>, List<DoctorStatisticDto>>(items)
            };
        }
        
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetPatientLookupAsync(LookupRequestDto input)
        {
            var query = (await patientRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.FirstName.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Patient>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Patient>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMedicalServiceLookupAsync(LookupRequestDto input)
        {
            var query = (await medicalServiceRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<MedicalService>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<MedicalService>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        
        [Authorize(HealthCarePermissions.Protocols.Delete)]
        public virtual async Task DeleteAsync(Guid id) =>  await protocolRepository.DeleteAsync(id);
        

        [Authorize(HealthCarePermissions.Protocols.Create)]
        public virtual async Task<ProtocolDto> CreateAsync(ProtocolCreateDto input)
        {
            var protocol = await protocolManager.CreateAsync(
             input.MedicalServiceNames,
            input.PatientId, input.DepartmentId, input.ProtocolTypeId, input.DoctorId, input.InsuranceId, input.StartTime, input.Notes, input.EndTime
            );
            
            await distributedEventBus.PublishAsync(new ProtocolsViewedEto { Id = protocol.Id, ViewedAt = Clock.Now },
                onUnitOfWorkComplete: false);

            return ObjectMapper.Map<Protocol, ProtocolDto>(protocol);
        }

        [Authorize(HealthCarePermissions.Protocols.Edit)]
        public virtual async Task<ProtocolDto> UpdateAsync(Guid id, ProtocolUpdateDto input)
        {
            
            var protocol = await protocolManager.UpdateAsync(
            id,  input.MedicalServices,
            input.PatientId, input.DepartmentId, input.ProtocolTypeId,  input.DoctorId,  input.InsuranceId, input.StartTime, input.Notes, input.EndTime, input.ConcurrencyStamp
            );
            
            await distributedEventBus.PublishAsync(new ProtocolsViewedEto { Id = protocol.Id, ViewedAt = Clock.Now },
                onUnitOfWorkComplete: false);

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
                Patient = item.Patient.FirstName + " " + item.Patient.LastName,
                Department = item.Department.Name,
                Doctor = item.Doctor.FirstName + " " + item.Doctor.LastName,
                ProtocolType = item.ProtocolType.Name,
                Insurance = item.Insurance.InsuranceCompanyName
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
        
        
        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new ProtocolDownloadTokenCacheItem { Token = token },
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