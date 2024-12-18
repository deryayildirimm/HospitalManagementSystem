using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.FamilyHistories;

public interface IFamilyHistoriesAppService
{
    Task<PagedResultDto<FamilyHistoryDto>> GetListAsync(GetFamilyHistoriesInput input);
    Task<FamilyHistoryDto> GetAsync(Guid id);
    Task<FamilyHistoryDto> CreateAsync(FamilyHistoryCreateDto input);
    Task<FamilyHistoryDto> UpdateAsync(FamilyHistoryUpdateDto input);
}