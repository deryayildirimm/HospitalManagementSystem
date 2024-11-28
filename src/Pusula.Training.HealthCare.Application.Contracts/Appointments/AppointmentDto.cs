using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentDto : FullAuditedEntityDto<Guid>
{
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public Guid PatientId { get; set; }
    public Guid AppointmentTypeId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; } 
    public EnumAppointmentStatus Status { get; protected set; }
    public string? Notes { get; protected set; } = string.Empty;
    public bool ReminderSent { get; protected set; }
    public double Amount { get; protected set; }
}