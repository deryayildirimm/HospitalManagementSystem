using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Exceptions;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Appointments;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Appointments.Default)]
public class AppointmentAppService(
    IAppointmentRepository appointmentRepository,
    IAppointmentManager appointmentManager,
    IDistributedCache<AppointmentDownloadTokenCacheItem, string> downloadTokenCache
) : HealthCareAppService, IAppointmentAppService
{
    public virtual async Task<PagedResultDto<AppointmentDayLookupDto>> GetAvailableDaysLookupAsync(
        GetAppointmentsLookupInput input)
    {
        var days = await appointmentManager
            .GetAvailableDaysLookupAsync(input.DoctorId, input.MedicalServiceId, input.StartDate, input.Offset);

        return new PagedResultDto<AppointmentDayLookupDto>(
            days.Count,
            days.ToList()
        );
    }

    public virtual async Task<PagedResultDto<AppointmentSlotDto>> GetAvailableSlotsAsync(GetAppointmentSlotInput input)
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

    public virtual async Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        var totalCount = await appointmentRepository.GetCountAsync(
            input.DoctorId, input.PatientId, input.MedicalServiceId,
            input.AppointmentMinDate, input.AppointmentMaxDate, input.StartTime, input.EndTime,
            input.Status, input.ReminderSent, input.MinAmount,
            input.MaxAmount);

        var items = await appointmentRepository.GetListAsync(
            input.DoctorId, input.PatientId, input.MedicalServiceId,
            input.AppointmentMinDate, input.AppointmentMaxDate, input.StartTime, input.EndTime,
            input.Status, input.ReminderSent, input.MinAmount,
            input.MaxAmount, input.Sorting, input.MaxResultCount, input.SkipCount);

        return new PagedResultDto<AppointmentDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(items)
        };
    }

    public virtual async Task<PagedResultDto<AppointmentWithNavigationPropertiesDto>>
        GetListWithNavigationPropertiesAsync(GetAppointmentsWithNavigationPropertiesInput input)
    {
        var totalCount = await appointmentRepository.GetCountByNavigationPropertiesAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount);

        var items = await appointmentRepository.GetListWithNavigationPropertiesAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount);

        return new PagedResultDto<AppointmentWithNavigationPropertiesDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper
                .Map<List<AppointmentWithNavigationProperties>, List<AppointmentWithNavigationPropertiesDto>>(items)
        };
    }

    public virtual async Task<AppointmentDto> GetAsync(Guid id)
        => ObjectMapper.Map<Appointment, AppointmentDto>(await appointmentRepository.GetAsync(id));

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteAsync(Guid id)
        => await appointmentRepository.DeleteAsync(id);

    public virtual async Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
    {
        try
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
        catch (AppointmentAlreadyTakenException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (AppointmentDateNotValidException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(@L["UnExpectedErrorOcurred"], ex);
        }
    }

    [Authorize(HealthCarePermissions.Appointments.Edit)]
    public virtual async Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        try
        {
            var appointment = await appointmentManager.UpdateAsync(
                id,
                input.AppointmentDate,
                input.StartTime,
                input.EndTime,
                input.Status,
                input.ReminderSent,
                input.Amount,
                input.Notes
            );

            return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
        }
        catch (AppointmentAlreadyTakenException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (AppointmentDateNotValidException ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new ApplicationException(@L["UnExpectedErrorOcurred"], ex);
        }
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await appointmentRepository.GetListWithNavigationPropertiesAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            input.AppointmentMinDate, 
            input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount,
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount);

        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(
            ObjectMapper.Map<List<AppointmentWithNavigationProperties>, List<AppointmentExcelDto>>(items));
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "Appointments.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> appointmentIds)
    {
        await appointmentRepository.DeleteManyAsync(appointmentIds);
    }

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteAllAsync(GetAppointmentsInput input)
    {
        await appointmentRepository.DeleteAllAsync(
            input.DoctorId, input.PatientId, input.MedicalServiceId,
            input.AppointmentMinDate, input.AppointmentMaxDate, input.StartTime, input.EndTime,
            input.Status, input.ReminderSent, input.MinAmount,
            input.MaxAmount);
    }

    public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        var token = Guid.NewGuid().ToString("N");

        await downloadTokenCache.SetAsync(
            token,
            new AppointmentDownloadTokenCacheItem { Token = token },
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            });

        return new DownloadTokenResultDto
        {
            Token = token
        };
    }
}