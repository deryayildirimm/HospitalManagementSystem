using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Titles;

public interface ITitlesAppService
{
    Task<PagedResultDto<TitleDto>> GetListAsync(GetTitlesInput input);
    Task<TitleDto> GetAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task<TitleDto> CreateAsync(TitleCreateDto input);
    Task<TitleDto> UpdateAsync(TitleUpdateDto input);
    Task DeleteByIdsAsync(List<Guid> titleIds);
    Task DeleteAllAsync(GetTitlesInput input);
}