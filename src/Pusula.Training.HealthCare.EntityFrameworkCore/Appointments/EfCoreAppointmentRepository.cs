using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.EntityFrameworkCore;
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
            doctorId, patientId, medicalServiceId, appointmentTypeId, departmentId, patientName, doctorName,
            serviceName,
            patientNumber, appointmentMinDate, appointmentMaxDate, startTime, endTime, status, patientType,
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
        var query = ApplyFilter((await GetQueryForNavigationPropertiesAsync()),
            doctorId, patientId, medicalServiceId, appointmentTypeId, departmentId, appointmentMinDate,
            appointmentMaxDate, startTime,
            endTime, status, reminderSent, minAmount, maxAmount);

        return await query.LongCountAsync(cancellationToken);
    }
    
    public virtual async Task<long> GetGroupCountByAsync(
        string groupByField,
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

    public virtual async Task<List<GroupedAppointmentCount>> GetGroupByListAsync(
        string groupByField,
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
            .OrderBy(d => d.GroupName)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }


    #region NavigationQueryCreator

    protected virtual async Task<IQueryable<Appointment>> GetQueryForNavigationPropertiesAsync()
        =>
            (await GetQueryableAsync())
            .Include(appointment => appointment.AppointmentType)
            .Include(appointment => appointment.Doctor)
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.MedicalService)
            .Include(appointment => appointment.Department);

    #endregion

    #region ApplyFilter

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
                x.Patient.FirstName.ToLower().Contains(patientName!.ToLower()) ||
                x.Patient.LastName.ToLower().Contains(patientName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(doctorName), x =>
                x.Doctor.FirstName.ToLower().Contains(doctorName!.ToLower()) ||
                x.Doctor.LastName.ToLower().Contains(doctorName!.ToLower()))
            .WhereIf(!string.IsNullOrWhiteSpace(serviceName), x =>
                x.MedicalService.Name.ToLower().Contains(serviceName!.ToLower()) ||
                x.MedicalService.Name.ToLower().Contains(serviceName!.ToLower()))
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

    private IQueryable<GroupedAppointmentCount> ApplyDynamicGrouping(IQueryable<Appointment> query, string groupByField)
    {
        if (groupByField == "Department")
        {
            return query.GroupBy(a => a.DepartmentId)
                .Select(g => new GroupedAppointmentCount
                {
                    GroupKey = g.Key.ToString(),
                    GroupName = g.First().Department.Name,
                    AppointmentCount = g.Count()
                });
        }

        if (groupByField == "Doctor")
        {
            return query.GroupBy(a => a.DoctorId)
                .Select(g => new GroupedAppointmentCount
                {
                    GroupKey = g.Key.ToString(),
                    GroupName =
                        $"{g.First().Doctor.Title.TitleName} {g.First().Doctor.FirstName} {g.First().Doctor.LastName}",
                    AppointmentCount = g.Count()
                });
        }

        if (groupByField == "Date")
        {
            return query.GroupBy(a => a.AppointmentDate.Date)
                .Select(g => new GroupedAppointmentCount
                {
                    GroupKey = g.Key.ToString(),
                    GroupName = g.Key.ToString(),
                    AppointmentCount = g.Count()
                });
        }

            //Group by department by default
        return query.GroupBy(a => a.DepartmentId)
            .Select(g => new GroupedAppointmentCount
            {
                GroupKey = g.Key.ToString(),
                GroupName = g.First().Department.Name,
                AppointmentCount = g.Count()
            });
    }

    #endregion
}