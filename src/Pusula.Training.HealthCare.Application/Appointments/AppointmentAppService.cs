using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Appointments;

[RemoteService(IsEnabled = false)]
[Authorize(HealthCarePermissions.Appointments.Default)]
public class AppointmentAppService(
    IAppointmentRepository appointmentRepository,
    IAppointmentManager appointmentManager,
    IDistributedCache<AppointmentDownloadTokenCacheItem, string> downloadTokenCache,
    IDistributedEventBus distributedEventBus
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
        var availableSlots = await appointmentManager
            .GetAppointmentSlotsAsync(input.DoctorId, input.MedicalServiceId, input.Date, input.ExcludeNotAvailable);

        return new PagedResultDto<AppointmentSlotDto>(
            availableSlots.Count,
            availableSlots.ToList()
        );
    }

    public virtual async Task<PagedResultDto<AppointmentDto>> GetListAsync(GetAppointmentsInput input)
    {
        var totalCount = await appointmentRepository.GetCountAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            appointmentTypeId: input.AppointmentTypeId,
            departmentId: input.DepartmentId,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount);

        var items = await appointmentRepository.GetListAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            appointmentTypeId: input.AppointmentTypeId,
            departmentId: input.DepartmentId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            patientType: input.PatientType,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<AppointmentDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Appointment>, List<AppointmentDto>>(items)
        };
    }

    public virtual async Task<PagedResultDto<GroupedAppointmentCountDto>> GetCountByGroupAsync(
        GetAppointmentsInput input)
    {
        var count = await appointmentRepository.GetGroupCountByAsync(
            groupByField: input.GroupByField,
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            appointmentTypeId: input.AppointmentTypeId,
            departmentId: input.DepartmentId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            patientType: input.PatientType,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount);

        var items = await appointmentRepository.GetGroupByListAsync(
            groupByField: input.GroupByField,
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            appointmentTypeId: input.AppointmentTypeId,
            departmentId: input.DepartmentId,
            patientName: input.PatientName,
            doctorName: input.DoctorName,
            serviceName: input.ServiceName,
            patientNumber: input.PatientNumber,
            appointmentMinDate: input.AppointmentMinDate,
            appointmentMaxDate: input.AppointmentMaxDate,
            startTime: input.StartTime,
            endTime: input.EndTime,
            status: input.Status,
            patientType: input.PatientType,
            reminderSent: input.ReminderSent,
            minAmount: input.MinAmount,
            maxAmount: input.MaxAmount,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);

        return new PagedResultDto<GroupedAppointmentCountDto>
        {
            TotalCount = count,
            Items = ObjectMapper.Map<List<GroupedAppointmentCount>, List<GroupedAppointmentCountDto>>(items)
        };
    }

    public virtual async Task<AppointmentDto> GetAsync(Guid id)
    {
        await distributedEventBus.PublishAsync(new AppointmentsViewedEto { Id = id, ViewedAt = Clock.Now },
            onUnitOfWorkComplete: false);
        return ObjectMapper.Map<Appointment, AppointmentDto>(await appointmentRepository.GetAsync(id));
    }

    public virtual async Task<AppointmentDto> GetByDateAsync(GetAppointmentByDateInput input)
    {
        var item = await appointmentRepository.GetByDateAsync(
            input.DoctorId,
            input.MedicalServiceId,
            input.StartTime,
            input.EndTime,
            input.AppointmentDate);

        await distributedEventBus.PublishAsync(new AppointmentsViewedEto { Id = item.Id, ViewedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        return ObjectMapper.Map<Appointment, AppointmentDto>(item);
    }

    [Authorize(HealthCarePermissions.Appointments.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await distributedEventBus.PublishAsync(new AppointmentDeletedEto { Id = id, ViewedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        await appointmentRepository.DeleteAsync(id);
    }

    public virtual async Task<AppointmentDto> CreateAsync(AppointmentCreateDto input)
    {
        var appointment = await appointmentManager.CreateAsync(
            input.DoctorId,
            input.PatientId,
            input.MedicalServiceId,
            input.AppointmentTypeId,
            input.DepartmentId,
            input.AppointmentDate,
            input.StartTime,
            input.EndTime,
            input.ReminderSent,
            input.Amount,
            input.Notes
        );

        await distributedEventBus.PublishAsync(new AppointmentCreatedEto { Id = appointment.Id, CreatedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

    [Authorize(HealthCarePermissions.Appointments.Edit)]
    public virtual async Task<AppointmentDto> UpdateAsync(Guid id, AppointmentUpdateDto input)
    {
        var appointment = await appointmentManager.UpdateAsync(
            id,
            input.DoctorId,
            input.AppointmentDate,
            input.StartTime,
            input.EndTime,
            input.Status,
            input.ReminderSent,
            input.Amount,
            input.Notes,
            input.CancellationNotes
        );

        await distributedEventBus.PublishAsync(new ApointmentUpdatedEto { Id = appointment.Id, UpdatedAt = Clock.Now },
            onUnitOfWorkComplete: false);

        return ObjectMapper.Map<Appointment, AppointmentDto>(appointment);
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(AppointmentExcelDownloadDto input)
    {
        var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
        if (downloadToken == null || input.DownloadToken != downloadToken.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
        }

        var items = await appointmentRepository.GetListAsync(
            doctorId: input.DoctorId,
            patientId: input.PatientId,
            medicalServiceId: input.MedicalServiceId,
            appointmentTypeId: input.AppointmentTypeId,
            departmentId: input.DepartmentId,
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
            ObjectMapper.Map<List<Appointment>, List<AppointmentExcelDto>>(items));
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
            input.AppointmentTypeId, input.DepartmentId, input.AppointmentMinDate, input.AppointmentMaxDate,
            input.StartTime, input.EndTime,
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