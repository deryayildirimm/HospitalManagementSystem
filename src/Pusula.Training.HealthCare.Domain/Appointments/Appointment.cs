using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Appointments;

public class Appointment : FullAuditedEntity<Guid>
{
    [NotNull]
    public virtual Guid DoctorId { get; protected set; }

    [NotNull] 
    public virtual Guid PatientId { get; protected set; }

    [NotNull]
    public virtual Guid MedicalServiceId { get; protected set; }

    [NotNull]
    public virtual DateTime AppointmentDate { get; protected set; }
    
    [NotNull]
    public DateTime StartTime { get; set; }

    [NotNull]
    public DateTime EndTime { get; set; } 

    [NotNull]
    public virtual EnumAppointmentStatus Status { get; protected set; }

    [CanBeNull]
    public virtual string? Notes { get; protected set; } = string.Empty;

    [NotNull]
    public virtual bool ReminderSent { get; protected set; }

    [NotNull] 
    public virtual double Amount { get; protected set; }

    protected Appointment()
    {
        Amount = 0.0;
        ReminderSent = false;
    }

    public Appointment(Guid id, Guid doctorId, Guid patientId, Guid medicalServiceId, DateTime appointmentDate,
        DateTime startTime, DateTime endTime, EnumAppointmentStatus status, string? notes, bool reminderSent, double amount)
    {
        Check.NotNull(id, nameof(id));
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNull(reminderSent, nameof(reminderSent));
        Check.Range(amount, nameof(amount), AppointmentConsts.MinAmount, AppointmentConsts.MaxAmount);

        if (startTime.TimeOfDay < AppointmentConsts.MinAppointmentTime ||
            startTime.TimeOfDay > AppointmentConsts.MaxAppointmentTime)
        {
            throw new ArgumentException(
                $"Appointment time must be between {AppointmentConsts.MinAppointmentTime} and {AppointmentConsts.MaxAppointmentTime}.");
        }
        
        if (endTime.TimeOfDay < AppointmentConsts.MinAppointmentTime ||
            endTime.TimeOfDay > AppointmentConsts.MaxAppointmentTime)
        {
            throw new ArgumentException(
                $"Appointment time must be between {AppointmentConsts.MinAppointmentTime} and {AppointmentConsts.MaxAppointmentTime}.");
        }

        if (!Array.Exists(AppointmentConsts.ValidStatuses, s => s == status))
        {
            throw new ArgumentException($"Invalid appointment status.");
        }

        if (!string.IsNullOrEmpty(notes) && notes.Length > AppointmentConsts.MaxNotesLength)
        {
            throw new ArgumentException($"Notes length cannot exceed {AppointmentConsts.MaxNotesLength} characters.");
        }

        Id = id;
        DoctorId = doctorId;
        PatientId = patientId;
        MedicalServiceId = medicalServiceId;
        AppointmentDate = appointmentDate;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
        ReminderSent = reminderSent;
        Amount = amount;
        Notes = notes;
    }
}