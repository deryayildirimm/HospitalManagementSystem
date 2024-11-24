using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Appointments;

public class Appointment : FullAuditedAggregateRoot<Guid>
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
        SetId(id);
        SetDoctorId(doctorId);
        SetPatientId(patientId);
        SetMedicalServiceId(medicalServiceId);
        SetAppointmentDate(appointmentDate);
        SetStartTime(startTime);
        SetEndTime(endTime);
        SetStatus(status);
        SetNotes(notes);
        SetReminderSent(reminderSent);
        SetAmount(amount);
    }
    
    public void SetId(Guid id)
    {
        Check.NotNull(id, nameof(id));
        Id = id;
    }

    private void SetDoctorId(Guid doctorId)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        DoctorId = doctorId;
    }

    public void SetPatientId(Guid patientId)
    {
        Check.NotNull(patientId, nameof(patientId));
        PatientId = patientId;
    }

    public void SetMedicalServiceId(Guid medicalServiceId)
    {
        Check.NotNull(medicalServiceId, nameof(medicalServiceId));
        MedicalServiceId = medicalServiceId;
    }

    public void SetAppointmentDate(DateTime appointmentDate)
    {
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        AppointmentDate = appointmentDate;
    }

    public void SetStartTime(DateTime startTime)
    {
        Check.NotNull(startTime, nameof(startTime));
        if (startTime.TimeOfDay < AppointmentConsts.MinAppointmentTime ||
            startTime.TimeOfDay > AppointmentConsts.MaxAppointmentTime)
        {
            throw new ArgumentException(
                $"Appointment time must be between {AppointmentConsts.MinAppointmentTime} and {AppointmentConsts.MaxAppointmentTime}.");
        }
        StartTime = startTime;
    }

    public void SetEndTime(DateTime endTime)
    {
        Check.NotNull(endTime, nameof(endTime));
        if (endTime.TimeOfDay < AppointmentConsts.MinAppointmentTime ||
            endTime.TimeOfDay > AppointmentConsts.MaxAppointmentTime)
        {
            throw new ArgumentException(
                $"Appointment time must be between {AppointmentConsts.MinAppointmentTime} and {AppointmentConsts.MaxAppointmentTime}.");
        }
        EndTime = endTime;
    }

    public void SetStatus(EnumAppointmentStatus status)
    {
        if (!Array.Exists(AppointmentConsts.ValidStatuses, s => s == status))
        {
            throw new ArgumentException($"Invalid appointment status.");
        }
        Status = status;
    }

    public void SetNotes(string? notes)
    {
        if (!string.IsNullOrEmpty(notes) && notes.Length > AppointmentConsts.MaxNotesLength)
        {
            throw new ArgumentException($"Notes length cannot exceed {AppointmentConsts.MaxNotesLength} characters.");
        }
        Notes = notes;
    }

    public void SetReminderSent(bool reminderSent)
    {
        Check.NotNull(reminderSent, nameof(reminderSent));
        ReminderSent = reminderSent;
    }

    public void SetAmount(double amount)
    {
        Check.NotNull(amount, nameof(amount));
        Check.Range(amount, nameof(amount), AppointmentConsts.MinAmount, AppointmentConsts.MaxAmount);
        Amount = amount;
    }
    
    
}