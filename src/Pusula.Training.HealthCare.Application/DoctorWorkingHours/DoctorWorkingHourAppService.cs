using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Doctors;
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
        var items = await workingHourRepository.GetListAsync(input.DoctorId, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<DoctorWorkingHoursDto>
        {
            TotalCount = items.Count,
            Items =
                ObjectMapper.Map<List<DoctorWorkingHour>, List<DoctorWorkingHoursDto>>(items)
        };
    }
}