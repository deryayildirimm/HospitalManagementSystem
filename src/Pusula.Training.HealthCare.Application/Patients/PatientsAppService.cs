using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Patients
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Patients.Default)]
    public class PatientsAppService(IPatientRepository patientRepository, PatientManager patientManager, 
        IDistributedCache<PatientDownloadTokenCacheItem, string> downloadTokenCache, 
        IDistributedEventBus distributedEventBus) : HealthCareAppService, IPatientsAppService
    {
        public virtual async Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input)
        {
            var totalCount = await patientRepository.GetCountAsync(input.FilterText, input.FirstName, input.LastName, input.BirthDateMin, input.BirthDateMax, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.HomePhoneNumber, input.GenderMin, input.GenderMax);
            var items = await patientRepository.GetListAsync(input.FilterText, input.FirstName, input.LastName, input.BirthDateMin, input.BirthDateMax, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.HomePhoneNumber, input.GenderMin, input.GenderMax, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<PatientDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Patient>, List<PatientDto>>(items)
            };
        }

        public virtual async Task<PatientDto> GetAsync(Guid id)
        {
            await distributedEventBus.PublishAsync(new PatientViewedEto { Id = id, ViewedAt = Clock.Now }, onUnitOfWorkComplete: false);

            var patient = await patientRepository.GetAsync(id);

            return ObjectMapper.Map<Patient, PatientDto>(patient);
        }

        [Authorize(HealthCarePermissions.Patients.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await patientRepository.DeleteAsync(id);
        }

        [Authorize(HealthCarePermissions.Patients.Create)]
        public virtual async Task<PatientDto> CreateAsync(PatientCreateDto input)
        {

            var patient = await patientManager.CreateAsync(
            input.FirstName, input.LastName, input.BirthDate, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.Gender, input.HomePhoneNumber
            );

            return ObjectMapper.Map<Patient, PatientDto>(patient);
        }

        [Authorize(HealthCarePermissions.Patients.Edit)]
        public virtual async Task<PatientDto> UpdateAsync(Guid id, PatientUpdateDto input)
        {

            var patient = await patientManager.UpdateAsync(
            id,
            input.FirstName, input.LastName, input.BirthDate, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.Gender, input.HomePhoneNumber, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Patient, PatientDto>(patient);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PatientExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await patientRepository.GetListAsync(input.FilterText, input.FirstName, input.LastName, input.BirthDateMin, input.BirthDateMax, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.HomePhoneNumber, input.GenderMin, input.GenderMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Patient>, List<PatientExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Patients.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Patients.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> patientIds)
        {
            await patientRepository.DeleteManyAsync(patientIds);
        }

        [Authorize(HealthCarePermissions.Patients.Delete)]
        public virtual async Task DeleteAllAsync(GetPatientsInput input)
        {
            await patientRepository.DeleteAllAsync(input.FilterText, input.FirstName, input.LastName, input.BirthDateMin, input.BirthDateMax, input.IdentityNumber, input.EmailAddress, input.MobilePhoneNumber, input.HomePhoneNumber, input.GenderMin, input.GenderMax);
        }

        public virtual async Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new PatientDownloadTokenCacheItem { Token = token },
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