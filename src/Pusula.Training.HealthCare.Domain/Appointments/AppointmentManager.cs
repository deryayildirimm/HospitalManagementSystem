using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentManager(
    IAppointmentRepository appointmentRepository,
    IRepository<DoctorWorkingHour> doctorWorkingHourRepository,
    IMedicalServiceRepository medicalServiceRepository) : DomainService, IAppointmentManager
{
    public virtual async Task<List<AppointmentSlotDto>> GetAppointmentSlotsAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime date)
    {
        //Check if working hours exist
        var workingHour = await GetDoctorWorkingHourAsync(doctorId, date);

        //Check if medical service exist
        var medicalService = await GetMedicalServiceAsync(medicalServiceId);

        //Check if doctor appointments exist
        var doctorAppointmentTimes = await GetDoctorAppointmentTimesAsync(doctorId, date);

        var slots = GenerateAppointmentSlots(
            startTime: workingHour.StartHour,
            endTime: workingHour.EndHour,
            durationMinutes: medicalService.Duration,
            date: date,
            skipPastSlots: true);

        return slots.Select(slot => new AppointmentSlotDto
        {
            DoctorId = doctorId,
            MedicalServiceId = medicalService.Id,
            Date = date,
            StartTime = slot.StartTime,
            EndTime = slot.EndTime,
            AvailabilityValue = !doctorAppointmentTimes.Any(appointment =>
                appointment.StartTime < TimeSpan.Parse(slot.EndTime) &&
                appointment.EndTime > TimeSpan.Parse(slot.StartTime))
        }).ToList();
    }

    public virtual async Task<List<AppointmentDayLookupDto>> GetAvailableDaysLookupAsync(
        Guid doctorId,
        Guid medicalServiceId,
        DateTime startDate,
        int offset)
    {
        var medicalService = await GetMedicalServiceAsync(medicalServiceId);
        var duration = medicalService.Duration;

        var appointments = (await GetAppointmentsAsync(doctorId, startDate, offset))
            .GroupBy(a => a.AppointmentDate.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        var workingHours = (await GetWorkingHoursAsync(doctorId))
            .ToDictionary(wh => wh.DayOfWeek, wh => wh);

        var slotsByDate = GetAppointmentSlotsByDate(workingHours, startDate, offset, duration);

        return slotsByDate
            .Select(item =>
            {
                var totalSlots = item.Value.Count;
                var isWeekend = item.Key.DayOfWeek.IsWeekend();

                //Calculate booked slot count
                var bookedSlots = appointments.TryGetValue(item.Key, out var value)
                    ? value.Count
                    : 0;

                var availableSlots = totalSlots - bookedSlots;
                
                //If the day is weekend then AvailabilityValue must be false
                return new AppointmentDayLookupDto
                {
                    Date = item.Key,
                    AvailableSlotCount = isWeekend ? 0 : availableSlots,
                    AvailabilityValue = !isWeekend && availableSlots > 0
                };
            })
            .ToList();
    }

    public virtual async Task<Appointment> CreateAsync(
        Guid doctorId,
        Guid patientId,
        Guid medicalServiceId,
        Guid appointmentTypeId,
        Guid departmentId,
        DateTime appointmentDate,
        DateTime startTime,
        DateTime endTime,
        bool reminderSent,
        double amount,
        string? notes = null)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(medicalServiceId, nameof(doctorId));
        Check.NotNull(departmentId, nameof(departmentId));
        Check.NotNull(appointmentTypeId, nameof(appointmentTypeId));
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNull(reminderSent, nameof(reminderSent));
        Check.NotNull(amount, nameof(amount));
        Check.Range(amount, nameof(amount), MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue);

        var alreadyHaveAppointment = await appointmentRepository.FirstOrDefaultAsync(
            x => x.PatientId == patientId
                 && x.AppointmentDate.Date == appointmentDate.Date
                 && x.StartTime >= startTime
                 && x.EndTime <= endTime) is not null;
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.AlreadyHaveAppointmentExactTime,
            alreadyHaveAppointment);

        var appointmentDateIsValid = startTime < DateTime.Now || endTime < DateTime.Now || startTime >= endTime;
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DateNotValid, appointmentDateIsValid);

        var isAppointmentTaken = await appointmentRepository.FirstOrDefaultAsync(
            x => x.DoctorId == doctorId
                 && x.AppointmentDate.Date == appointmentDate.Date
                 && x.StartTime == startTime);
        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.AppointmentAlreadyTaken,
            isAppointmentTaken is not null);

        var appointment = new Appointment(
            id: GuidGenerator.Create(),
            doctorId: doctorId,
            patientId: patientId,
            medicalServiceId: medicalServiceId,
            appointmentTypeId: appointmentTypeId,
            departmentId: departmentId,
            appointmentDate: appointmentDate,
            startTime: startTime,
            endTime: endTime,
            status: EnumAppointmentStatus.Scheduled,
            notes: notes,
            reminderSent: reminderSent,
            amount: amount
        );

        return await appointmentRepository.InsertAsync(appointment);
    }

    public virtual async Task<Appointment> UpdateAsync(
        Guid id,
        DateTime appointmentDate,
        DateTime startTime,
        DateTime endTime,
        EnumAppointmentStatus status,
        bool reminderSent, double amount,
        [CanBeNull] string? notes = null)
    {
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNull(reminderSent, nameof(reminderSent));
        Check.NotNull(amount, nameof(amount));
        Check.Range(amount, nameof(amount), MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue);
        Check.Range((int)status, nameof(status), AppointmentConsts.StatusMinValue, AppointmentConsts.StatusMaxValue);

        var appointment = await appointmentRepository.GetAsync(id);

        appointment.SetAppointmentDate(appointmentDate);
        appointment.SetStartTime(startTime);
        appointment.SetEndTime(endTime);
        appointment.SetStatus(status);
        appointment.SetNotes(notes);
        appointment.SetReminderSent(reminderSent);
        appointment.SetAmount(amount);

        return await appointmentRepository.UpdateAsync(appointment);
    }

    private async Task<DoctorWorkingHour> GetDoctorWorkingHourAsync(Guid doctorId, DateTime date)
    {
        var workingHour = await doctorWorkingHourRepository
            .FirstOrDefaultAsync(x => x.DoctorId == doctorId && date.DayOfWeek == x.DayOfWeek);

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.DoctorWorkingHourNotFound,
            workingHour is null);

        return workingHour!;
    }

    private async Task<MedicalService> GetMedicalServiceAsync(Guid medicalServiceId)
    {
        var medicalService = await medicalServiceRepository
            .FirstOrDefaultAsync(x => x.Id == medicalServiceId);

        HealthCareGlobalException.ThrowIf(HealthCareDomainErrorKeyValuePairs.MedicalServiceNotFound,
            medicalService is null);

        return medicalService!;
    }

    private async Task<List<(TimeSpan StartTime, TimeSpan EndTime)>> GetDoctorAppointmentTimesAsync(Guid doctorId,
        DateTime date)
    {
        return (await appointmentRepository
                .GetListAsync(x => x.DoctorId == doctorId && x.AppointmentDate.Date == date.Date))
            .Select(x => (StartTime: x.StartTime.TimeOfDay, EndTime: x.EndTime.TimeOfDay))
            .ToList();
    }

    private async Task<List<Appointment>> GetAppointmentsAsync(Guid doctorId, DateTime startDate, int offset)
    {
        return await appointmentRepository
            .GetListAsync(x => x.DoctorId == doctorId
                               && x.AppointmentDate.Date >= startDate.Date
                               && x.AppointmentDate.Date < startDate.Date.AddDays(offset));
    }

    private static Dictionary<DateTime, List<AppointmentSlotBaseDto>> GetAppointmentSlotsByDate(
        Dictionary<DayOfWeek, DoctorWorkingHour> workingHours,
        DateTime startDate,
        int offset,
        int duration)
    {
        return Enumerable.Range(0, offset)
            .Select(i => startDate.AddDays(i)) // Iterate over each day starting from start date
            .SelectMany<DateTime, AppointmentSlotBaseDto>(curDate =>
                workingHours.TryGetValue(curDate.DayOfWeek, out var workingHour)
                    ? GenerateAppointmentSlots(workingHour.StartHour, workingHour.EndHour, duration, curDate, false)
                    : [new AppointmentSlotBaseDto { Date = curDate }]
            )
            .GroupBy(slot => slot.Date) // Group by date
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    private async Task<List<DoctorWorkingHour>> GetWorkingHoursAsync(Guid doctorId)
    {
        return await doctorWorkingHourRepository
            .GetListAsync(x => x.DoctorId == doctorId);
    }

    private static List<AppointmentSlotBaseDto> GenerateAppointmentSlots(
        TimeOnly startTime,
        TimeOnly endTime,
        int durationMinutes,
        DateTime date,
        bool skipPastSlots = true)
    {
        //Slot uzunlugu hesaplanir 
        var serviceDuration = TimeSpan.FromMinutes(durationMinutes);

        return Enumerable
            .Range(0,
                (int)((endTime.ToTimeSpan() - startTime.ToTimeSpan()).TotalMinutes / serviceDuration.TotalMinutes))
            .Select(i => startTime.AddMinutes(i * durationMinutes))
            .Where(appointmentTime =>
            {
                //ayni gun icinde gecmis slotlari gormemek adina yapilan kontrol
                var isSameDay = date.Date == DateTime.Now.Date;
                var isPastSlot = appointmentTime < TimeOnly.FromDateTime(DateTime.Now);
                return !(skipPastSlots && isSameDay && isPastSlot);
            })
            .Select(appointmentTime => new AppointmentSlotBaseDto
            {
                Date = date.Date,
                StartTime = appointmentTime.ToString("HH:mm"),
                EndTime = appointmentTime.AddMinutes(durationMinutes).ToString("HH:mm")
            })
            .ToList();
    }
}