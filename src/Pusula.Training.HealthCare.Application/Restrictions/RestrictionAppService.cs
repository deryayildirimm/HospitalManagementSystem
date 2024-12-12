using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Restrictions;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.MedicalServices.Default)]
public class RestrictionAppService(
    IRestrictionRepository restrictionRepository,
    IRestrictionManager restrictionManager
) : HealthCareAppService, IRestrictionAppService
{
    public virtual async Task<PagedResultDto<RestrictionDto>> GetListAsync(GetRestrictionsInput input)
    {
        var totalCount = await restrictionRepository.GetCountAsync(
            doctorId: input.DoctorId,
            medicalServiceId: input.MedicalServiceId,
            departmentId: input.DepartmentId);

        var items = await restrictionRepository.GetListAsync(
            medicalServiceId: input.MedicalServiceId,
            departmentId: input.DepartmentId,
            doctorId: input.DoctorId,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<RestrictionDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Restriction>, List<RestrictionDto>>(items)
        };
    }

    public virtual async Task<RestrictionDto> GetAsync(Guid id)
        => ObjectMapper.Map<Restriction, RestrictionDto>(await restrictionRepository.GetAsync(id));

    public virtual async Task DeleteAsync(Guid id)
        => await restrictionRepository.DeleteAsync(id);

    [Authorize(HealthCarePermissions.MedicalServices.Create)]
    public virtual async Task<RestrictionDto> CreateAsync(RestrictionCreateDto input)
    {
        var restriction = await restrictionManager.CreateAsync(
            input.MedicalServiceId,
            input.DepartmentId,
            input.DoctorId,
            input.MinAge,
            input.MaxAge,
            input.AllowedGender
        );

        return ObjectMapper.Map<Restriction, RestrictionDto>(restriction);
    }

    [Authorize(HealthCarePermissions.MedicalServices.Edit)]
    public virtual async Task<RestrictionDto> UpdateAsync(Guid id, RestrictionUpdateDto input)
    {
        var restriction = await restrictionManager.UpdateAsync(
            id,
            input.MedicalServiceId,
            input.DepartmentId,
            input.DoctorId,
            input.MinAge,
            input.MaxAge,
            input.AllowedGender
        );

        return ObjectMapper.Map<Restriction, RestrictionDto>(restriction);
    }

    [Authorize(HealthCarePermissions.MedicalServices.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> ids)
        => await restrictionRepository.DeleteManyAsync(ids);
}