using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.Patients;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentsInput : PagedAndSortedResultRequestDto
{
    public string GroupByField { get; set; } = string.Empty;
    public Guid? DoctorId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? MedicalServiceId { get; set; }
    public Guid? AppointmentTypeId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? PatientName { get; set; }
    public string? DoctorName { get; set; }
    public string? ServiceName { get; set; }
    public int? PatientNumber { get; set; }
    public DateTime? AppointmentMinDate { get; set; }
    public DateTime? AppointmentMaxDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EnumPatientTypes? PatientType { get; set; }
    public EnumAppointmentStatus? Status { get; set; }
    public bool? ReminderSent { get; set; }
    public double? MinAmount { get; set; }
    public double? MaxAmount { get; set; }

    public bool Condition { get; set; } = true;
}