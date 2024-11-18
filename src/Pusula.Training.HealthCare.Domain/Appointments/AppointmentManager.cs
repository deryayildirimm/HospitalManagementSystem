using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Exceptions;
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

    public async Task<List<AppointmentSlotDto>> GetAppointmentSlotsAsync(Guid doctorId, Guid medicalServiceId, DateTime date)
    {
        var workingHour = await doctorWorkingHourRepository
            .FirstOrDefaultAsync(x => x.DoctorId == doctorId && date.DayOfWeek == x.DayOfWeek);

        if (workingHour == null)
        {
            throw new DoctorNotWorkingException();
        }

        var medicalService = await medicalServiceRepository
            .FirstOrDefaultAsync(x => x.Id == medicalServiceId);

        if (medicalService == null)
        {
            throw new MedicalServiceNotFoundException();
        }

        var doctorAppointments = await appointmentRepository
            .GetListAsync(x => x.DoctorId == doctorId && x.AppointmentDate.Date == date);

        var startTime = workingHour.StartHour;
        var endTime = workingHour.EndHour;
        var serviceDuration = TimeSpan.FromMinutes(medicalService.Duration);

        var availableSlots = new List<AppointmentSlotDto>();

        for (var appointmentTime = startTime;
             appointmentTime + serviceDuration <= endTime;
             appointmentTime += serviceDuration)
        {
            var isAvailable = !doctorAppointments.Any(x =>
                x.AppointmentDate.Date == date &&
                x.AppointmentTime.TimeOfDay == appointmentTime);

            availableSlots.Add(new AppointmentSlotDto
            {
                Date = date,
                StartTime = appointmentTime.ToString(@"hh\:mm"),
                EndTime = appointmentTime.Add(serviceDuration).ToString(@"hh\:mm"),
                DoctorId = doctorId,
                MedicalServiceId = medicalService.Id,
                AvailabilityValue = isAvailable,
            });
        }

        return availableSlots;
    }
}