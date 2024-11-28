using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Exceptions;
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
        var workingHour = await GetDoctorWorkingHourAsync(doctorId, date);
        var medicalService = await GetMedicalServiceAsync(medicalServiceId);
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

        if (startDate < DateTime.Now.Date)
        {
            throw new DateNotInPastException();
        }
        
        //Check if medical service exist
        var medicalService = await medicalServiceRepository
            .FirstOrDefaultAsync(x => x.Id == medicalServiceId);

        if (medicalService == null)
        {
            throw new MedicalServiceNotFoundException();
        }

        var doctorAppointments = await appointmentRepository
            .GetListAsync(x => x.DoctorId == doctorId
                               && x.AppointmentDate.Date >= startDate.Date
                               && x.AppointmentDate.Date < startDate.Date.AddDays(offset));

        // Group appointments by dates
        var appointmentsByDate = doctorAppointments
            .GroupBy(x => x.AppointmentDate.Date)
            .ToDictionary(g => g.Key, g => g.Select(a => new { a.StartTime, a.EndTime }).ToList());

        // Get doctor's working hours
        var workingHours = await doctorWorkingHourRepository
            .GetListAsync(x => x.DoctorId == doctorId);

        // Generate slots for everyday
        var serviceDuration = TimeSpan.FromMinutes(medicalService.Duration);
        var availableDays = new List<AppointmentDayLookupDto>();

        for (var i = 0; i < offset; i++)
        {
            var currentDate = startDate.AddDays(i);
            var dayOfWeek = currentDate.DayOfWeek;

            // Check if doctor works on the selected day
            var workingHour = workingHours.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
            if (workingHour == null)
            {

                availableDays.Add(new AppointmentDayLookupDto
                {
                    Date = currentDate,
                    DoctorId = doctorId,
                    MedicalServiceId = medicalService.Id,
                    AvailableSlotCount = 0
                });
                continue;
            }

            var startTime = workingHour.StartHour;
            var endTime = workingHour.EndHour;

            appointmentsByDate.TryGetValue(key: currentDate.Date, out var appointmentsForTheDay);
            var availableSlotCount = 0;

            //Generate slots for a specific doctor
            for (var appointmentTime = startTime; appointmentTime + serviceDuration <= endTime; appointmentTime += serviceDuration)
            {
                
                if (currentDate.Date == DateTime.Now.Date && appointmentTime < DateTime.Now.TimeOfDay)
                {
                    continue;
                }

                if (appointmentsForTheDay == null || !appointmentsForTheDay.Any(a => appointmentTime >= a.StartTime.TimeOfDay && appointmentTime < a.EndTime.TimeOfDay))
                {
                    availableSlotCount++;
                }
            }
            
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
        DateTime appointmentDate, DateTime startTime,
        DateTime endTime, bool reminderSent, double amount, string? notes = null)
    {
        Check.NotNull(doctorId, nameof(doctorId));
        Check.NotNull(patientId, nameof(patientId));
        Check.NotNull(medicalServiceId, nameof(doctorId));
        Check.NotNull(appointmentTypeId, nameof(appointmentTypeId));
        Check.NotNull(appointmentDate, nameof(appointmentDate));
        Check.NotNull(startTime, nameof(startTime));
        Check.NotNull(endTime, nameof(endTime));
        Check.NotNull(reminderSent, nameof(reminderSent));
        Check.NotNull(amount, nameof(amount));
        Check.Range(amount, nameof(amount), MedicalServiceConsts.CostMinValue, MedicalServiceConsts.CostMaxValue);

        if (startTime < DateTime.Now || endTime < DateTime.Now || startTime >= endTime)
        {
            throw new AppointmentDateNotValidException();
        }

        var isAppointmentTaken = await appointmentRepository
            .FirstOrDefaultAsync(x => x.DoctorId == doctorId
                                      && x.AppointmentDate.Date == appointmentDate.Date
                                      && x.StartTime == startTime);

        if (isAppointmentTaken != null)
        {
            throw new AppointmentAlreadyTakenException();
        }

        var appointment = new Appointment(
            id: GuidGenerator.Create(),
            doctorId: doctorId,
            patientId: patientId,
            medicalServiceId: medicalServiceId,
            appointmentTypeId: appointmentTypeId,
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

        if (workingHour == null)
        {
            throw new DoctorNotWorkingException();
        }

        return workingHour;
    }

    private async Task<MedicalService> GetMedicalServiceAsync(Guid medicalServiceId)
    {
        var medicalService = await medicalServiceRepository
            .FirstOrDefaultAsync(x => x.Id == medicalServiceId);

        if (medicalService == null)
        {
            throw new MedicalServiceNotFoundException();
        }

        return medicalService;
    }

    private async Task<List<(TimeSpan StartTime, TimeSpan EndTime)>> GetDoctorAppointmentTimesAsync(Guid doctorId,
        DateTime date)
    {
        return (await appointmentRepository
                .GetListAsync(x => x.DoctorId == doctorId && x.AppointmentDate.Date == date.Date))
            .Select(x => (StartTime: x.StartTime.TimeOfDay, EndTime: x.EndTime.TimeOfDay))
            .ToList();
    }
    
    private async Task<List<Appointment>> GetAppointments(Guid doctorId, DateTime startDate, int offset)
    {
        return await appointmentRepository
            .GetListAsync(x => x.DoctorId == doctorId
                               && x.AppointmentDate.Date >= startDate.Date
                               && x.AppointmentDate.Date < startDate.Date.AddDays(offset));
    }

    private List<AppointmentSlotBaseDto> GenerateAppointmentSlots(
        TimeSpan startTime,
        TimeSpan endTime,
        int durationMinutes,
        DateTime date,
        int offset,
        bool skipPastSlots = true)
    {
        var slots = new List<AppointmentSlotBaseDto>();
        var serviceDuration = TimeSpan.FromMinutes(durationMinutes);

        for (var appointmentTime = startTime;
             appointmentTime + serviceDuration <= endTime;
             appointmentTime += serviceDuration)
        {
            if (skipPastSlots && date.Date == DateTime.Now.Date && appointmentTime < DateTime.Now.TimeOfDay)
            {
                continue;
            }

            slots.Add(new AppointmentSlotBaseDto
            {
                Date = date,
                StartTime = appointmentTime.ToString(@"hh\:mm"),
                EndTime = appointmentTime.Add(serviceDuration).ToString(@"hh\:mm"),
            });
        }

        return slots;
    }
}