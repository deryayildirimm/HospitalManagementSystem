using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentDto : FullAuditedEntityDto<Guid>
{
    public Guid DoctorId { get; set; }
    public Guid MedicalServiceId { get; set; }
    public  Guid PatientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; } 
    public virtual EnumAppointmentStatus Status { get; protected set; }
    public virtual string? Notes { get; protected set; } = string.Empty;
    public virtual bool ReminderSent { get; protected set; }
    public virtual double Amount { get; protected set; }
}