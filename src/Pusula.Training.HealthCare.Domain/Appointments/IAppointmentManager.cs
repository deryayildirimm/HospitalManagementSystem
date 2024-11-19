using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Appointments;

public interface IAppointmentManager
{
    Task<List<AppointmentSlotDto>> GetAppointmentSlotsAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime date
    );

    Task<Appointment> CreateAsync(
        Guid doctorId, 
        Guid patientId, 
        Guid medicalServiceId, 
        DateTime appointmentDate,
        DateTime startTime,
        DateTime endTime,
        bool reminderSent,
        double amount,
        string? notes = null
    );
}