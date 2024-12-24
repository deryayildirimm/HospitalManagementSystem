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
        Task<PagedResultDto<BloodTestDto>> GetListAsync(GetBloodTestsInput input);
        Task<BloodTestDto> GetAsync(Guid id);
        Task<BloodTestDto> CreateAsync(BloodTestCreateDto input);
        Task<BloodTestDto> UpdateAsync(BloodTestUpdateDto input);
        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BloodTestExcelDownloadDto input);
        Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<List<Guid>> GetCategoryIdsAsync(Guid id);
    }
}
