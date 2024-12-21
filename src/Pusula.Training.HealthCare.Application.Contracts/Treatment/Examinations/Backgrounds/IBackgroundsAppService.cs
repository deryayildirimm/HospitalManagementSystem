using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.Backgrounds;

public interface IBackgroundsAppService
{
    Task<PagedResultDto<BackgroundDto>> GetListAsync(GetBackgroundsInput input);
    Task<BackgroundDto> GetAsync(Guid id);
    Task<BackgroundDto> CreateAsync(BackgroundCreateDto input);
    Task<BackgroundDto> UpdateAsync(BackgroundUpdateDto input);
}