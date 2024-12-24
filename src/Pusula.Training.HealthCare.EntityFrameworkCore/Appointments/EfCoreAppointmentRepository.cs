using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using Pusula.Training.HealthCare.GlobalExceptions;
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
        Guid? departmentId = null,
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
            departmentId,
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

    public virtual async Task<Appointment> GetByDateAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime startTime,
        DateTime endTime,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()),
            doctorId: doctorId,
            medicalServiceId: medicalServiceId,
            appointmentDate: appointmentDate,
            startTime: startTime,
            endTime: endTime);

        var appointment = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.AppointmentNotFound, appointment is null);
        return appointment!;
    }

    public virtual async Task<List<Appointment>> GetListAsync(
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        Guid? departmentId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        EnumPatientTypes? patientType = null,
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
            doctorId,
            patientId,
            medicalServiceId,
            appointmentTypeId,
            departmentId,
            patientName,
            doctorName,
            serviceName,
            patientNumber,
            appointmentMinDate,
            appointmentMaxDate,
            startTime,
            endTime,
            status,
            patientType,
            reminderSent,
            minAmount,
            maxAmount);

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
        Guid? departmentId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        EnumPatientTypes? patientType = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            doctorId,
            patientId,
            medicalServiceId,
            appointmentTypeId,
            departmentId,
            patientName,
            doctorName,
            serviceName,
            patientNumber,
            appointmentMinDate,
            appointmentMaxDate,
            startTime,
            endTime,
            status,
            patientType,
            reminderSent,
            minAmount,
            maxAmount);

        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<long> GetGroupCountByAsync(
        EnumAppointmentGroupFilter groupByField,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        Guid? departmentId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        EnumPatientTypes? patientType = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(
            (await GetQueryForNavigationPropertiesAsync()),
            doctorId, patientId, medicalServiceId, appointmentTypeId, departmentId, patientName, doctorName,
            serviceName,
            patientNumber, appointmentMinDate, appointmentMaxDate, startTime, endTime, status, patientType,
            reminderSent,
            minAmount, maxAmount);

        var groupedQuery = ApplyDynamicGrouping(query, groupByField);

        return await groupedQuery.LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<AppointmentStatistic>> GetGroupByListAsync(
        EnumAppointmentGroupFilter groupByField,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        Guid? departmentId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        EnumPatientTypes? patientType = null,
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
            doctorId, patientId, medicalServiceId, appointmentTypeId, departmentId, patientName, doctorName,
            serviceName,
            patientNumber, appointmentMinDate, appointmentMaxDate, startTime, endTime, status, patientType,
            reminderSent,
            minAmount, maxAmount);

        var groupedQuery = ApplyDynamicGrouping(query, groupByField);

        return await groupedQuery
            .OrderBy(string.IsNullOrWhiteSpace(sorting)
                ? AppointmentConsts.GetGroupDefaultSorting(false)
                : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<Appointment>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(appointment => appointment.AppointmentType)
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Doctor.Title)
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.MedicalService)
            .Include(appointment => appointment.Department);

    #endregion

    #region ApplyFilter

    protected virtual IQueryable<Appointment> ApplyFilter(
        IQueryable<Appointment> query,
        Guid doctorId,
        Guid medicalServiceId,
        DateTime startTime,
        DateTime endTime,
        DateTime appointmentDate) =>
        query
            .Where(x => x.DoctorId == doctorId)
            .Where(x => x.MedicalServiceId == medicalServiceId)
            .Where(x => x.AppointmentDate.Date == appointmentDate.Date)
            .Where(x => x.StartTime >= startTime && x.EndTime <= endTime);

    protected virtual IQueryable<Appointment> ApplyFilter(
        IQueryable<Appointment> query,
        Guid? doctorId = null,
        Guid? patientId = null,
        Guid? medicalServiceId = null,
        Guid? appointmentTypeId = null,
        Guid? departmentId = null,
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
            .WhereIf(departmentId.HasValue, x => x.DepartmentId == departmentId)
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
        Guid? departmentId = null,
        string? patientName = null,
        string? doctorName = null,
        string? serviceName = null,
        int? patientNumber = null,
        DateTime? appointmentMinDate = null,
        DateTime? appointmentMaxDate = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EnumAppointmentStatus? status = null,
        EnumPatientTypes? patientType = null,
        bool? reminderSent = null,
        double? minAmount = null,
        double? maxAmount = null) =>
        query
            .WhereIf(doctorId.HasValue, x => x.DoctorId == doctorId)
            .WhereIf(patientId.HasValue, x => x.PatientId == patientId)
            .WhereIf(medicalServiceId.HasValue, x => x.MedicalServiceId == medicalServiceId)
            .WhereIf(appointmentTypeId.HasValue, e => e.AppointmentTypeId == appointmentTypeId)
            .WhereIf(departmentId.HasValue, x => x.DepartmentId == departmentId)
            .WhereIf(!string.IsNullOrWhiteSpace(patientName), x =>
                EF.Functions.ILike(x.Patient.FirstName, $"%{patientName}%") ||
                EF.Functions.ILike(x.Patient.LastName, $"%{patientName}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(doctorName), x =>
                EF.Functions.ILike(x.Doctor.FirstName, $"%{doctorName}%") ||
                EF.Functions.ILike(x.Doctor.LastName, $"%{doctorName}%"))
            .WhereIf(!string.IsNullOrWhiteSpace(serviceName), x =>
                EF.Functions.ILike(x.MedicalService.Name, $"%{serviceName}%") ||
                EF.Functions.ILike(x.Doctor.LastName, $"%{serviceName}%"))
            .WhereIf(patientNumber.HasValue, x => x.Patient.PatientNumber == patientNumber)
            .WhereIf(patientType.HasValue, x => x.Patient.PatientType == patientType)
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

    #region DynamicGroupByQuery

    private static IQueryable<AppointmentStatistic> GroupAppointments(
        IQueryable<Appointment> query,
        Expression<Func<Appointment, object>> groupKey,
        Func<Appointment, string> groupFunc,
        bool appointmentCountSum = false)
        => query
            .GroupBy(groupKey)
            .Select(g => new AppointmentStatistic
            {
                GroupKey = FormatKey(g.Key),
                GroupName = groupFunc(g.FirstOrDefault()!),
                Number = appointmentCountSum ? (int)g.Sum(x => x.Amount) : g.Count()
            });

    private static string FormatKey<TKey>(TKey key) => (key switch
    {
        DateTime dateKey => dateKey.ToShortDateString(),
        _ => key!.ToString()
    })!;

    private static IQueryable<AppointmentStatistic> ApplyDynamicGrouping(
        IQueryable<Appointment> query,
        EnumAppointmentGroupFilter groupByField)
    {
        return groupByField switch
        {
            EnumAppointmentGroupFilter.Service => GroupAppointments(query, a => a.MedicalServiceId,
                a => a.MedicalService.Name),

            EnumAppointmentGroupFilter.Doctor => GroupAppointments(query, a => a.DoctorId,
                a => $"{a.Doctor.Title.TitleName} {a.Doctor.FirstName} {a.Doctor.LastName}"),

            EnumAppointmentGroupFilter.Status => GroupAppointments(query, a => a.Status,
                a => a.Status.ToString()),

            EnumAppointmentGroupFilter.Date => GroupAppointments(query, a => a.AppointmentDate.Date,
                a => a.AppointmentDate.Date.ToShortDateString()),

            EnumAppointmentGroupFilter.AppointmentType => GroupAppointments(query, a => a.AppointmentTypeId,
                a => a.AppointmentType.Name),

            EnumAppointmentGroupFilter.RevenueByService => GroupAppointments(query, a => a.MedicalServiceId,
                a => a.MedicalService.Name, true),

            EnumAppointmentGroupFilter.RevenueByDepartment => GroupAppointments(query, a => a.DepartmentId,
                a => a.Department.Name, true),

            EnumAppointmentGroupFilter.PatientGender => GroupAppointments(query, a => a.Patient.Gender,
                a => a.Patient.Gender.ToString()),
            _ => GroupAppointments(query, a => a.DepartmentId,
                a => a.Department.Name),
        };
    }

    #endregion
}