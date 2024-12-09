using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Doctors.Default)]
public class DoctorWorkingHourAppService(IDoctorWorkingHourRepository workingHourRepository)
    : HealthCareAppService, IDoctorWorkingHourAppService

{
    public virtual async Task<PagedResultDto<DoctorWorkingHoursDto>> GetListAsync(GetDoctorWorkingHoursInput input)
    {
        var items = await workingHourRepository.GetListAsync(input.DoctorId, input.Sorting, input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<DoctorWorkingHoursDto>
        {
            TotalCount = items.Count,
            Items =
                ObjectMapper.Map<List<DoctorWorkingHour>, List<DoctorWorkingHoursDto>>(items)
        };
    }

    public virtual async Task<DoctorWorkingHoursDto> GetAsync(Guid id) =>
        ObjectMapper.Map<DoctorWorkingHour, DoctorWorkingHoursDto>(await workingHourRepository.GetAsync(id));

    [Authorize(HealthCarePermissions.Doctors.Delete)]
    public virtual async Task DeleteAsync(Guid id) => await workingHourRepository.DeleteAsync(id);

    [Authorize(HealthCarePermissions.Doctors.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> ids) => await workingHourRepository.DeleteManyAsync(ids);
}