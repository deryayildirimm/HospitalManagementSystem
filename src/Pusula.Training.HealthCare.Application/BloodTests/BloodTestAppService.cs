using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
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
    public class BloodTestAppService(
        IBloodTestRepository bloodTestRepository,
        IBloodTestManager bloodTestManager,
        IDistributedCache<BloodTestDownloadTokenCacheItem, string> downloadTokenCache) : HealthCareAppService, IBloodTestAppService
    {
        public virtual async Task<PagedResultDto<BloodTestWithNavigationPropertiesDto>> GetListAsync(GetBloodTestsInput input)
        {
            var totalCount = await bloodTestRepository.GetCountAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax,
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId, input.TestCategoryId);
            var items = await bloodTestRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax, 
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId, input.TestCategoryId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BloodTestWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BloodTestWithNavigationProperties>, List<BloodTestWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<BloodTestWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
            => ObjectMapper.Map<BloodTestWithNavigationProperties, BloodTestWithNavigationPropertiesDto>(await bloodTestRepository.GetWithNavigationPropertiesAsync(id));

        public virtual async Task<BloodTestDto> GetAsync(Guid id) => ObjectMapper.Map<BloodTest, BloodTestDto>(await bloodTestRepository.GetAsync(id));


        [Authorize(HealthCarePermissions.BloodTests.Create)]
        public virtual async Task<BloodTestDto> CreateAsync(BloodTestCreateDto input)
        {
            if (input.DoctorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Doctor"]]);
            }
            if (input.PatientId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Patient"]]);
            }
            if (input.TestCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TestCategory"]]);
            }

            var bloodTest = await bloodTestManager.CreateAsync(input.DoctorId, input.PatientId, input.TestCategoryId, input.Status, input.DateCreated, input.DateCompleted);

            return ObjectMapper.Map<BloodTest, BloodTestDto>(bloodTest);
        }

        [Authorize(HealthCarePermissions.BloodTests.Edit)]
        public virtual async Task<BloodTestDto> UpdateAsync(BloodTestUpdateDto input)
        {
            if (input.DoctorId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Doctor"]]);
            }
            if (input.PatientId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Patient"]]);
            }
            if (input.TestCategoryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["TestCategory"]]);
            }

            var bloodTest = await bloodTestManager.UpdateAsync(input.Id, input.DoctorId, input.PatientId, input.Status, input.DateCreated, input.DateCompleted);

            return ObjectMapper.Map<BloodTest, BloodTestDto>(bloodTest);
        }

        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BloodTestExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var bloodTest = await bloodTestRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Status, input.DateCreatedMin, input.DateCreatedMax, 
                input.DateCompletedMin, input.DateCompletedMax, input.DoctorId, input.PatientId, input.TestCategoryId);
            var items = bloodTest.Select(e => new
            {
                e.BloodTest.DateCreated,
                e.BloodTest.DateCompleted,
                Doctor = e.Doctor?.FirstName,
                Patient = e.Patient?.FirstName,
                Category = e.TestCategory?.Name,
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

        public async Task BulkUpdateStatusAsync(List<BloodTestUpdateDto> updateDtos)
        {
            foreach (var dto in updateDtos)
            {
                await bloodTestManager.UpdateAsync(
                    dto.Id,              
                    dto.DoctorId,        
                    dto.PatientId,       
                    dto.Status,          
                    dto.DateCreated,     
                    dto.DateCompleted    
                );
            }
        }


    }
}
