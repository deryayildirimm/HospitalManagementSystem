using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

public interface IPhysicalFindingsAppService
{
    
    Task<PagedResultDto<PhysicalFindingDto>> GetListAsync(GetPhysicalFindingsInput input);
    Task<PhysicalFindingDto> GetAsync(Guid id);
    Task<PhysicalFindingDto> CreateAsync(PhysicalFindingCreateDto input);
    Task<PhysicalFindingDto> UpdateAsync(PhysicalFindingUpdateDto input);
}