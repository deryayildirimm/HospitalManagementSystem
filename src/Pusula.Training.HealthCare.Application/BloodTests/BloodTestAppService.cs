using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
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

namespace Pusula.Training.HealthCare.BloodTests
{
    [RemoteService(IsEnabled =false)]
    [Authorize(HealthCarePermissions.BloodTests.Default)]
    public class BloodTestAppService(
        IBloodTestRepository bloodTestRepository,
        IBloodTestManager bloodTestManager,
        IDistributedCache<BloodTestDownloadTokenCacheItem, string> downloadTokenCache) : HealthCareAppService, IBloodTestAppService
    {
        
        public virtual async Task<PagedResultDto<BloodTestDto>> GetListAsync(GetBloodTestsInput input)
        {
            var totalCount = await bloodTestRepository.GetCountAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax,
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId);
            var items = await bloodTestRepository.GetListAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax,
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId, input.Sorting, input.MaxResultCount, input.SkipCount);
            return new PagedResultDto<BloodTestDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BloodTest>, List<BloodTestDto>>(items)
            };
        }
        
        public virtual async Task<BloodTestDto> GetAsync(Guid id)
        {
            var result = await bloodTestRepository.GetWithNavigationPropertiesAsync(id);
            return ObjectMapper.Map<BloodTest, BloodTestDto>(result!);
        }

        [Authorize(HealthCarePermissions.BloodTests.Create)]
        public virtual async Task<BloodTestDto> CreateAsync(BloodTestCreateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorInformationsRequired, input.DoctorId == default);
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.PatientInformationsRequired, input.PatientId == default);

            var bloodTest = await bloodTestManager.CreateAsync(input.DoctorId, input.PatientId, input.Status, input.DateCreated, input.DateCompleted, input.TestCategoryIdList);

            return ObjectMapper.Map<BloodTest, BloodTestDto>(bloodTest);
        }

        [Authorize(HealthCarePermissions.BloodTests.Edit)]
        public virtual async Task<BloodTestDto> UpdateAsync(BloodTestUpdateDto input)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorInformationsRequired,input.DoctorId == default);
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.PatientInformationsRequired,input.PatientId == default);

            var bloodTest = await bloodTestManager.UpdateAsync(input.Id, input.DoctorId, input.PatientId, input.Status, input.DateCreated, input.DateCompleted, input.TestCategoryIdList);

            return ObjectMapper.Map<BloodTest, BloodTestDto>(bloodTest);
        }

        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BloodTestExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var bloodTest = await bloodTestRepository.GetListAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax, 
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId);
            var items = bloodTest.Select(e => new
            {
                e.BloodTestCategories,
                Doctor = e.Doctor?.FirstName,
                Patient = e.Patient?.FirstName,
            });
            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "BloodTest.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new BloodTestDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }

        public async Task<List<Guid>> GetCategoryIdsAsync(Guid id)
        {
            HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.CategoryInformationsRequired, id == default);

            return await bloodTestRepository.GetCategoryIdsAsync(id);
        }        
    }
}
