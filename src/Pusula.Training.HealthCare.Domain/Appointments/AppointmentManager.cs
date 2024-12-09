using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Exceptions;
using Pusula.Training.HealthCare.GlobalExceptions;
using Pusula.Training.HealthCare.MedicalServices;
using Volo.Abp;
using Volo.Abp.Data;
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
            offset: 1,
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

    public virtual async Task<List<AppointmentDayLookupDto>> GetAvailableDaysLookupAsync(Guid doctorId,
        Guid medicalServiceId, DateTime startDate, int offset)
    {
        //Check if medical service exist
        var medicalService = await GetMedicalServiceAsync(medicalServiceId);

        //Get doctor's appointments
        var appointments = await GetAppointmentsAsync(doctorId, startDate, offset);

        //Get doctor's working hours
        var workingHours = await GetWorkingHoursAsync(doctorId);

        var availableDays = new List<AppointmentDayLookupDto>();

        for (var i = 0; i < offset; i++)
        {
            var currentDate = startDate.AddDays(i);
            var dayOfWeek = currentDate.DayOfWeek;

            var workingHour = workingHours.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
            if (workingHour == null)
            {
                availableDays.Add(new AppointmentDayLookupDto
                {
                    Date = currentDate,
                    DoctorId = doctorId,
                    MedicalServiceId = medicalServiceId,
                    AvailableSlotCount = 0,
                    AvailabilityValue = false
                });
                continue;
            }

            var appointmentsForTheDay = appointments
                .Where(a => a.AppointmentDate.Date == currentDate.Date)
                .ToList();

            var allPossibleSlots = GenerateAppointmentSlots(
                workingHour.StartHour,
                workingHour.EndHour,
                medicalService.Duration,
                currentDate,
                offset);

            var availableSlotCount = allPossibleSlots.Count(slot =>
                !appointmentsForTheDay.Any(a =>
                    TimeSpan.Parse(slot.StartTime) >= a.StartTime.TimeOfDay &&
                    TimeSpan.Parse(slot.StartTime) < a.EndTime.TimeOfDay));

            availableDays.Add(new AppointmentDayLookupDto
            {
                Date = currentDate.Date,
                DoctorId = doctorId,
                MedicalServiceId = medicalService.Id,
                AvailableSlotCount = availableSlotCount,
                AvailabilityValue = availableSlotCount > 0
            });
        }

        return availableDays;
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
    
    private async Task<List<DoctorWorkingHour>> GetWorkingHoursAsync(Guid doctorId)
    {
        return await doctorWorkingHourRepository
            .GetListAsync(x => x.DoctorId == doctorId);
    }

    private static List<AppointmentSlotBaseDto> GenerateAppointmentSlots(
        TimeSpan startTime,
        TimeSpan endTime,
        int durationMinutes,
        DateTime date,
        int offset,
        bool skipPastSlots = true)
    {
        var serviceDuration = TimeSpan.FromMinutes(durationMinutes);

        return Enumerable
            .Range(0, (int)((endTime - startTime).TotalMinutes / serviceDuration.TotalMinutes))
            .Select(i => startTime + TimeSpan.FromMinutes(i * durationMinutes))
            .Where(appointmentTime =>
                !(skipPastSlots && date.Date == DateTime.Now.Date && appointmentTime < DateTime.Now.TimeOfDay))
            .Select(appointmentTime => new AppointmentSlotBaseDto
            {
                Date = date,
                StartTime = appointmentTime.ToString(@"hh\:mm"),
                EndTime = (appointmentTime + serviceDuration).ToString(@"hh\:mm")
            })
            .ToList();
    }
    
}