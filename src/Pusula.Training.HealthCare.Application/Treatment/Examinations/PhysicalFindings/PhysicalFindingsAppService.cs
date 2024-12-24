using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Treatment.Examinations.PhysicalFindings;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.PhysicalFindings.Default)]
public class PhysicalFindingsAppService(IPhysicalFindingRepository physicalFindingRepository,
    PhysicalFindingManager physicalFindingManager) : HealthCareAppService, IPhysicalFindingsAppService
{
    public virtual async Task<PagedResultDto<PhysicalFindingDto>> GetListAsync(GetPhysicalFindingsInput input)
    {
        var totalCount = await physicalFindingRepository.GetCountAsync(input.WeightMin, input.WeightMax, input.HeightMin,
            input.HeightMax, input.ExaminationId);
        var items = await physicalFindingRepository.GetListAsync(input.WeightMin, input.WeightMax, 
            input.HeightMin, input.HeightMax, input.ExaminationId, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<PhysicalFindingDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<PhysicalFinding>, List<PhysicalFindingDto>>(items)
        };
    }

    public virtual async Task<PhysicalFindingDto> GetAsync(Guid id)
    {
        var physicalFinding = await physicalFindingRepository.GetAsync(id);
        
        return ObjectMapper.Map<PhysicalFinding, PhysicalFindingDto>(physicalFinding);
    }

    [Authorize(HealthCarePermissions.PhysicalFindings.Create)]
    public virtual async Task<PhysicalFindingDto> CreateAsync(PhysicalFindingCreateDto input)
    {
        var physicalFinding = await physicalFindingManager.CreateAsync(input.ExaminationId, input.Weight, 
            input.Height, input.BodyTemperature, input.Pulse, input.Vki, input.Vya, input.Kbs, input.Kbd,
            input.Spo2);
        
        return ObjectMapper.Map<PhysicalFinding, PhysicalFindingDto>(physicalFinding);
    }

    [Authorize(HealthCarePermissions.PhysicalFindings.Edit)]
    public virtual async Task<PhysicalFindingDto> UpdateAsync(PhysicalFindingUpdateDto input)
    {
        var physicalFinding = await physicalFindingManager.UpdateAsync(input.Id, input.ExaminationId, input.Weight, 
            input.Height, input.BodyTemperature, input.Pulse, input.Vki, input.Vya, input.Kbs, input.Kbd,
            input.Spo2);
        
        return ObjectMapper.Map<PhysicalFinding, PhysicalFindingDto>(physicalFinding);
    }
}