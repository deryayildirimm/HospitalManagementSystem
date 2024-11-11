using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Exceptions;
using Pusula.Training.HealthCare.Permissions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Patients
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Patients.Default)]
    public class PatientsAppService(IPatientRepository patientRepository, PatientManager patientManager,
        IDistributedCache<PatientDownloadTokenCacheItem, string> downloadTokenCache,
        IDistributedEventBus distributedEventBus,
        IDataFilter _dataFilter) : HealthCareAppService, IPatientsAppService
    {
        public virtual async Task<PagedResultDto<PatientDto>> GetListAsync(GetPatientsInput input)
        {
            // ISoftDelete filtresini IsDeleted durumuna göre devre dışı bırak veya etkinleştir
            using (_dataFilter.Disable<ISoftDelete>())
            {
                var totalCount = await patientRepository.GetCountAsync(input.FilterText, input.PatientNumber, input.FirstName,
                    input.LastName, input.IdentityNumber,
                    input.Nationality, input.PassportNumber, input.BirthDateMin, input.BirthDateMax, input.EmailAddress,
                    input.MobilePhoneNumber,
                    input.PatientType, input.InsuranceType, input.InsuranceNo, input.DiscountGroup, input.Gender);
                var items = await patientRepository.GetListAsync(input.FilterText, input.PatientNumber, input.FirstName, input.LastName,
                    input.IdentityNumber,
                    input.Nationality, input.PassportNumber, input.BirthDateMin, input.BirthDateMax, input.EmailAddress,
                    input.MobilePhoneNumber,
                    input.PatientType, input.InsuranceType, input.InsuranceNo, input.DiscountGroup, input.Gender);

                if (input.IsDeleted == true)
                {
                    items = items.Where(x => x.IsDeleted == input.IsDeleted).ToList();
                }

                return new PagedResultDto<PatientDto>
                {
                    TotalCount = totalCount,
                    Items = ObjectMapper.Map<List<Patient>, List<PatientDto>>(items)
                };
            }
        }

        public virtual async Task<PatientDto> GetAsync(Guid id)
        {
            using (_dataFilter.Disable<ISoftDelete>())
            {

                await distributedEventBus.PublishAsync(new PatientViewedEto { Id = id, ViewedAt = Clock.Now },
                    onUnitOfWorkComplete: false);

                var patient = await patientRepository.GetAsync(id);

                return ObjectMapper.Map<Patient, PatientDto>(patient);
            }
        }

        [Authorize(HealthCarePermissions.Patients.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await patientRepository.DeleteAsync(id);
        }

        [Authorize(HealthCarePermissions.Patients.Create)]
        public virtual async Task<PatientDto> CreateAsync(PatientCreateDto input)
        {

            try
            {
                var existingPatient = (await patientRepository.GetListAsync()).FirstOrDefault(p => (p.IdentityNumber == input.IdentityNumber && p.IdentityNumber != null) ||
                                                                                                  (p.PassportNumber == input.PassportNumber && p.PassportNumber != null));

                if (existingPatient != null)
                {
                    throw new PatientAlreadyExistsException();
                }

                int patientNumber = (await patientRepository.GetListAsync()).Count == 0
                    ? 1
                    : ((await patientRepository.GetListAsync()).LastOrDefault()!.PatientNumber) + 1;

                var patient = await patientManager.CreateAsync(
                    patientNumber,
                    input.FirstName, input.LastName, input.Nationality, input.BirthDate,
                    input.MobilePhoneNumber, input.PatientType, input.InsuranceType, input.InsuranceNo, input.Gender,
                    input.MothersName, input.FathersName, input.IdentityNumber, input.PassportNumber,
                    input.EmailAddress, input.Relative, input.RelativePhoneNumber, input.Address, input.DiscountGroup
                );
                return ObjectMapper.Map<Patient, PatientDto>(patient);
            }
            catch (PatientAlreadyExistsException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            catch (Exception ex)
            {

                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        [Authorize(HealthCarePermissions.Patients.Edit)]
        public virtual async Task<PatientDto> UpdateAsync(Guid id, PatientUpdateDto input)
        {
            try
            {
                var existingPatient = (await patientRepository.GetListAsync()).FirstOrDefault(p => (p.IdentityNumber == input.IdentityNumber && p.IdentityNumber != null) ||
                                                                                                  (p.PassportNumber == input.PassportNumber && p.PassportNumber != null));

                if (existingPatient != null)
                {
                    throw new PatientAlreadyExistsException();
                }

                var patient = await patientManager.UpdateAsync(
            id,
            input.FirstName, input.LastName, input.Nationality, input.BirthDate,
            input.MobilePhoneNumber, input.PatientType, input.InsuranceType, input.InsuranceNo, input.Gender, input.IsDeleted, input.MothersName,
            input.FathersName, input.IdentityNumber, input.PassportNumber, input.EmailAddress, input.Relative, input.RelativePhoneNumber, input.Address, input.DiscountGroup
            );

                return ObjectMapper.Map<Patient, PatientDto>(patient);
            }
            catch (PatientAlreadyExistsException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            catch (PatientUpdateException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(PatientExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await patientRepository.GetListAsync(input.FilterText, input.PatientNumber, input.FirstName, input.LastName, input.IdentityNumber,
                input.Nationality, input.PassportNumber, input.BirthDateMin, input.BirthDateMax, input.EmailAddress, input.MobilePhoneNumber,
                input.PatientType, input.InsuranceType, input.InsuranceNo, input.DiscountGroup, input.Gender);

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
            await patientRepository.DeleteAllAsync(input.FilterText, input.PatientNumber, input.FirstName, input.LastName, input.IdentityNumber,
                input.Nationality, input.PassportNumber, input.BirthDateMin, input.BirthDateMax, input.EmailAddress, input.MobilePhoneNumber,
                input.PatientType, input.InsuranceType, input.InsuranceNo, input.DiscountGroup, input.Gender);
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