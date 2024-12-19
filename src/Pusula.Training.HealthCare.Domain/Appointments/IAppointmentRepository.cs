using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Domain.Repositories; 

namespace Pusula.Training.HealthCare.Appointments;

public interface IAppointmentRepository : IRepository<Appointment, Guid>
{
    Task DeleteAllAsync(
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
        CancellationToken cancellationToken = default);
    
    Task<Appointment> GetByDateAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime startTime,
        DateTime endTime,
        DateTime appointmentDate,
        CancellationToken cancellationToken = default);
    
    Task<List<Appointment>> GetListAsync(
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
        CancellationToken cancellationToken = default);
    
    Task<long> GetCountAsync(
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
        CancellationToken cancellationToken = default);
    
    Task<long> GetGroupCountByAsync(
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
        CancellationToken cancellationToken = default);
    
    Task<List<GroupedAppointmentCount>> GetGroupByListAsync(
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
        CancellationToken cancellationToken = default);
    
    
}