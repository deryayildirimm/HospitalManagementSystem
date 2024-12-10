using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class GetAppointmentsWithNavigationPropertiesInput : PagedAndSortedResultRequestDto
{
    public Guid? DoctorId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? MedicalServiceId { get; set; }
    public Guid? AppointmentTypeId { get; set; }
    public string? PatientName { get; set; }
    public string? DoctorName { get; set; }
    public string? ServiceName { get; set; }
    public int? PatientNumber { get; set; }
    public DateTime? AppointmentMinDate { get; set; }
    public DateTime? AppointmentMaxDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public EnumAppointmentStatus? Status { get; set; }
    public bool? ReminderSent { get; set; }
    public double? MinAmount { get; set; }
    public double? MaxAmount { get; set; }
}