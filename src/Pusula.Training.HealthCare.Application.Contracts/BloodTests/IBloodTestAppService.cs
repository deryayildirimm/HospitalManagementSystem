using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.BloodTests
{
    public interface IBloodTestAppService : IApplicationService
    {
        Task<PagedResultDto<BloodTestWithNavigationPropertiesDto>> GetListAsync(GetBloodTestsInput input);

        Task<BloodTestWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<BloodTestDto> GetAsync(Guid id);

        Task<BloodTestDto> CreateAsync(BloodTestCreateDto input);

        Task<BloodTestDto> UpdateAsync(BloodTestUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BloodTestExcelDownloadDto input);
        Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
        Task BulkUpdateStatusAsync(List<BloodTestUpdateDto> updateDtos);

    }
}
