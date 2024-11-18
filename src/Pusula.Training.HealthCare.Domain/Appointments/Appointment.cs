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
    public virtual DateTime AppointmentTime { get; protected set; }

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

    public Appointment(Guid doctorId, Guid patientId, Guid medicalServiceId, DateTime appointmentDate,
        DateTime appointmentTime, EnumAppointmentStatus status, string? notes, bool reminderSent, double amount)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        Check.NotNull(appointmentTime, nameof(appointmentTime));
        Check.NotNull(appointmentTime, nameof(appointmentTime));
        Check.NotNull(reminderSent, nameof(reminderSent));
        Check.Range(amount, nameof(amount), AppointmentConsts.MinAmount, AppointmentConsts.MaxAmount);

        if (appointmentTime.TimeOfDay < AppointmentConsts.MinAppointmentTime ||
            appointmentTime.TimeOfDay > AppointmentConsts.MaxAppointmentTime)
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

        DoctorId = doctorId;
        PatientId = patientId;
        MedicalServiceId = medicalServiceId;
        AppointmentDate = appointmentDate;
        AppointmentTime = appointmentTime;
        Status = status;
        ReminderSent = reminderSent;
        Amount = amount;
        Notes = notes;
    }
}