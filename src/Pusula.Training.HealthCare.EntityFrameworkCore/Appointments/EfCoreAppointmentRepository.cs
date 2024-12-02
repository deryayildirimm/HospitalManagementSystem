using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.AppointmentTypes;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Appointments;

public class EfCoreAppointmentRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
    : EfCoreRepository<HealthCareDbContext, Appointment, Guid>(dbContextProvider), IAppointmentRepository
{
    public virtual async Task DeleteAllAsync(
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryableAsync()),
            doctorId,
            patientId,
            medicalServiceId,
            appointmentTypeId,
            appointmentMinDate,
            appointmentMaxDate,
            startTime,
            endTime,
            status,
            reminderSent,
            minAmount,
            maxAmount);

        var ids = query.Select(x => x.Id);
        await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Appointment>> GetListAsync(
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null,
        string? sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            doctorId, patientId, medicalServiceId, appointmentTypeId, patientName, doctorName, serviceName,
            patientNumber, appointmentMinDate, appointmentMaxDate, startTime, endTime, status,
            reminderSent,
            minAmount, maxAmount);

        query = query.OrderBy(string.IsNullOrWhiteSpace(sorting)
            ? AppointmentConsts.GetDefaultSorting(false)
            : sorting);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }


    public virtual async Task<long> GetCountAsync(
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()),
            doctorId, patientId, medicalServiceId, appointmentTypeId, appointmentMinDate, appointmentMaxDate, startTime,
            endTime, status, reminderSent, minAmount, maxAmount);

        return await query.LongCountAsync(cancellationToken);
    }

    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<Appointment>> GetQueryForNavigationPropertiesAsync()
        =>
        (await GetDbContextAsync()).Set<Appointment>()
            .Include(appointment => appointment.AppointmentType)
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.MedicalService);
    
    #endregion

    #region ApplyFilter
    protected virtual IQueryable<Appointment> ApplyFilter(
        IQueryable<Appointment> query,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null) =>
        query
            .WhereIf(doctorId.HasValue, x => x.DoctorId == doctorId)
            .WhereIf(patientId.HasValue, x => x.PatientId == patientId)
            .WhereIf(medicalServiceId.HasValue, x => x.MedicalServiceId == medicalServiceId)
            .WhereIf(appointmentTypeId.HasValue, x => x.AppointmentTypeId == appointmentTypeId)
            .WhereIf(appointmentMinDate.HasValue, e => e.AppointmentDate.Date >= appointmentMinDate!.Value.Date)
            .WhereIf(appointmentMaxDate.HasValue, e => e.AppointmentDate.Date <= appointmentMaxDate!.Value.Date)
            .WhereIf(startTime.HasValue, e => e.StartTime.Date == startTime)
            .WhereIf(endTime.HasValue, e => e.EndTime == endTime)
            .WhereIf(status.HasValue, e => e.Status == status)
            .WhereIf(reminderSent.HasValue, e => e.ReminderSent == reminderSent)
            .WhereIf(minAmount.HasValue,
                e => e.Amount >= minAmount!.Value)
            .WhereIf(maxAmount.HasValue,
                e => e.Amount <= maxAmount!.Value);
    
    protected virtual IQueryable<Appointment> ApplyFilter(
        IQueryable<Appointment> query,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null) =>
        query
            .WhereIf(doctorId.HasValue, x => x.DoctorId == doctorId)
            .WhereIf(patientId.HasValue, x => x.PatientId == patientId)
            .WhereIf(medicalServiceId.HasValue, x => x.MedicalServiceId == medicalServiceId)
            .WhereIf(appointmentTypeId.HasValue, e => e.AppointmentTypeId == appointmentTypeId)
            .WhereIf(!string.IsNullOrWhiteSpace(patientName), x =>
                x.Patient.FirstName!.ToLower().Contains(patientName!.ToLower()) ||
                x.Patient.LastName!.ToLower().Contains(patientName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(doctorName), x =>
                x.Doctor.FirstName!.ToLower().Contains(doctorName!.ToLower()) ||
                x.Doctor.LastName!.ToLower().Contains(doctorName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(serviceName),
                x => x.MedicalService.Name!.ToLower().Contains(serviceName!.ToLower()))
            .WhereIf(patientNumber.HasValue, x => x.Patient.PatientNumber == patientNumber)
            .WhereIf(appointmentMinDate.HasValue,
                e => e.AppointmentDate.Date >= appointmentMinDate!.Value.Date)
            .WhereIf(appointmentMaxDate.HasValue,
                e => e.AppointmentDate.Date <= appointmentMaxDate!.Value.Date)
            .WhereIf(startTime.HasValue, e => e.StartTime.Date == startTime)
            .WhereIf(endTime.HasValue, e => e.EndTime == endTime)
            .WhereIf(status.HasValue, e => e.Status == status)
            .WhereIf(reminderSent.HasValue, e => e.ReminderSent == reminderSent)
            .WhereIf(minAmount.HasValue,
                e => e.Amount >= minAmount!.Value)
            .WhereIf(maxAmount.HasValue,
                e => e.Amount <= maxAmount!.Value);

    #endregion
}