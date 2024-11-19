using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Exceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Appointments;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Appointments.Default)]
public class AppointmentAppService(
    IRepository<DoctorWorkingHour> doctorWorkingHourRepository,
    IAppointmentRepository appointmentRepository,
    IMedicalServiceRepository medicalServiceRepository,
    IAppointmentManager appointmentManager,
    IDistributedCache<MedicalServiceDownloadTokenCacheItem, string> downloadCache
) : HealthCareAppService, IAppointmentAppService
{
    public async Task<PagedResultDto<AppointmentSlotDto>> GetAvailableSlotsAsync(GetAppointmentsInput input)
    {
        try
        {
            var availableSlots = await appointmentManager
                .GetAppointmentSlotsAsync(input.DoctorId, input.MedicalServiceId, input.Date);
            
            return new PagedResultDto<AppointmentSlotDto>(
                availableSlots.Count,
                availableSlots.ToList()
            );
        }
        catch (MedicalServiceNotFoundException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (DoctorNotWorkingException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(@L["UnExpectedErrorOcurred"], ex);
        }
    }

    [Authorize(HealthCarePermissions.Departments.Edit)]
    public Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    public Task<AppointmentDto> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
    {
        var appointment = await appointmentManager.CreateAsync(
            input.DoctorId,
            input.PatientId,
            input.MedicalServiceId,
            input.AppointmentDate,
            input.StartTime,
            input.EndTime,
            input.ReminderSent,
            input.Amount,
            input.Notes
            );
        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

    public Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        throw new NotImplementedException();
    }

    public Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        throw new NotImplementedException();
    }

    public Task DeleteByIdsAsync(List<Guid> appointmentIds)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllAsync(GetAppointmentsInput input)
    {
        throw new NotImplementedException();
    }

    public Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        throw new NotImplementedException();
    }
}