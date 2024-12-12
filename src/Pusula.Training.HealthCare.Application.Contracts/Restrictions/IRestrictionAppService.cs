using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Training.HealthCare.Restrictions;

public interface IRestrictionAppService : IApplicationService
{
    Task<PagedResultDto<RestrictionDto>> GetListAsync(GetRestrictionsInput input);
    
    Task<RestrictionDto> GetAsync(Guid id);
    
    Task DeleteAsync(Guid id);

    Task<RestrictionDto> CreateAsync(RestrictionCreateDto input);

    Task<RestrictionDto> UpdateAsync(Guid id, RestrictionUpdateDto input);
    
    Task DeleteByIdsAsync(List<Guid> ids);
}