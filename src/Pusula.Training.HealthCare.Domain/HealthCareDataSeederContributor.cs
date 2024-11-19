﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace Pusula.Training.HealthCare
{
    public class HealthCareDataSeederContributor(
        IdentityRoleManager roleManager,
        IPermissionManager permissionManager,
        IdentityUserManager userManager,
        IPatientRepository patientRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        ITitleRepository titleRepository,
        IMedicalServiceRepository medicalServiceRepository,
        IRepository<Doctor> doctorRepository,
        IRepository<DoctorWorkingHour> doctorWorkingHourRepository,
        IGuidGenerator guidGenerator) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            await SetRoles();
        }

        private async Task SetRoles()
        {
            await SeedPatientRecords();
            await SeedRoleRecords();
            await SeedMedicalServiceRecords();
            await SeedDepartmentRecords();
            await SeedMedicalServiceToDepartments();
            await SeedTitles();
            await SeedDoctorRecords();
            await SeedDoctorWorkingHours();
            await SeedAppointments();
        }

        private async Task SeedMedicalServiceRecords()
        {
            if (await medicalServiceRepository.GetCountAsync() > 0)
                return;

            await medicalServiceRepository.InsertAsync(
                new MedicalService(Guid.NewGuid(), "X-Ray", 300.00, 20, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "MRI Scan", 1200.00, 20,
                DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Cardiology Consultation",
                250.00, 20, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Pediatric Check-up",
                120.00, 20, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(
                new MedicalService(Guid.NewGuid(), "Chiropractic Session", 90.00, 20, DateTime.Now));
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Nutrition Consultation",
                75.00, 20, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Psychiatric Evaluation",
                300.0, 25, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Radiology Review", 910.00,
                20, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Physical Therapy Assessment",
                725.00, 10, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Obstetrics Ultrasound",
                400.00, 20, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Emergency Room Visit",
                500.00, 20, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Examination",
                100.00, 10, DateTime.Now), true);
        }

        private async Task SeedDepartmentRecords()
        {
            if (await departmentRepository.GetCountAsync() > 0)
                return;

            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Cardiology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Radiology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Emergency"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Pediatrics"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Orthopedics"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Dermatology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Urology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Oncology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Neurology"), true);
            await departmentRepository.InsertAsync(new Department(Guid.NewGuid(), "Dermatology"), true);
        }

        private async Task SeedMedicalServiceToDepartments()
        {
            if (await medicalServiceRepository.GetCountAsync() == 0
                || await medicalServiceRepository.GetCountAsync() == 0)
                return;

            var medicalServices = await medicalServiceRepository.GetListAsync();
            var departments = await departmentRepository.GetListAsync();

            foreach (var department in departments)
            {
                var random = new Random();

                var randomServices = medicalServices.OrderBy(ms => random.Next()).Take(2).ToList();
                foreach (var service in randomServices)
                {
                    var departmentMedicalService = new DepartmentMedicalService
                    {
                        MedicalServiceId = service.Id,
                        DepartmentId = department.Id
                    };

                    service.DepartmentMedicalServices.Add(departmentMedicalService);
                }
            }

            await departmentRepository.UpdateManyAsync(departments, true);
        }

        private async Task SeedPatientRecords()
        {
            if (await patientRepository.GetCountAsync() > 0)
                return;

            var patient1 = new Patient(
                Guid.NewGuid(),
                1,
                "Ali",
                "Yılmaz",
                "TURKISH",
                new DateTime(1990, 1, 1),
                "12345678901",
                EnumPatientTypes.VIP,
                EnumInsuranceType.SGK,
                "A12345678",
                EnumGender.MALE,
                "Fatma",
                "Aykut",
                "INS123456",
                "A1234567",
                "ali.yilmaz@example.com",
                EnumRelative.FATHER,
                "12344678901",
                "İstanbul",
                EnumDiscountGroup.STAFF
            );

            var patient2 = new Patient(
                Guid.NewGuid(),
                2,
                "Mehmet",
                "Demir",
                "ENGLISH",
                new DateTime(1985, 5, 10),
                "98765432109",
                EnumPatientTypes.NORMAL,
                EnumInsuranceType.PRIVATE,
                "B98765432",
                EnumGender.MALE,
                "Emine",
                "Suat",
                "INS987654",
                "B7654321",
                "mehmet.demir@example.com",
                EnumRelative.MOTHER,
                "09876543210",
                "Ankara",
                EnumDiscountGroup.STAFF
            );

            var patient3 = new Patient(
                Guid.NewGuid(),
                4,
                "Ayşe",
                "Kaya",
                "German",
                new DateTime(1992, 3, 15),
                "12312312345",
                EnumPatientTypes.VIP,
                EnumInsuranceType.SGK,
                "C12312312",
                EnumGender.FEMALE,
                "Hatice",
                "Ahmet",
                "INS123123",
                "C1231234",
                "ayse.kaya@example.com",
                EnumRelative.FATHER,
                "11223344556",
                "Izmir",
                EnumDiscountGroup.CONTRACTED
            );

            await patientRepository.InsertManyAsync([patient1, patient2, patient3], true);
        }

        private async Task SeedTitles()
        {
            var titles = new List<Title>
            {
                new Title(guidGenerator.Create(), "Dr."),
                new Title(guidGenerator.Create(), "Prof."),
                new Title(guidGenerator.Create(), "Yrd. Doç."),
                new Title(guidGenerator.Create(), "Op."),
            };

            await titleRepository.InsertManyAsync(titles, autoSave: true);
        }

        private async Task SeedDoctorRecords()
        {
            if (await titleRepository.GetCountAsync() == 0 || await departmentRepository.GetCountAsync() == 0)
                return;

            var random = new Random();
            var titles = await titleRepository.GetListAsync();
            var departments = await departmentRepository.GetListAsync();

            var d1 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Arif",
                "Yılmaz",
                "12345678901",
                new DateTime(1980, 5, 12),
                EnumGender.MALE,
                15,
                "İstanbul",
                "Kadıköy",
                "ahmet.yilmaz@example.com",
                "555-1234567"
            );

            var d2 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Fatma",
                "Kara",
                "98765432109",
                new DateTime(1990, 3, 25),
                EnumGender.FEMALE,
                5,
                "Ankara",
                "Çankaya",
                "fatma.kara@example.com",
                "555-9876543"
            );

            var d3 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Mehmet",
                "Çelik",
                "12309876543",
                new DateTime(1975, 11, 30),
                EnumGender.MALE,
                20,
                "İzmir",
                "Konak",
                "mehmet.celik@example.com",
                "555-3219876"
            );

            var d4 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[1].Id,
                "Merve",
                "Şahin",
                "23456789012",
                new DateTime(1985, 8, 23),
                EnumGender.FEMALE,
                12,
                "Ankara",
                "Çankaya",
                "merve.sahin@example.com",
                "555-2345678"
            );

            var d5 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[1].Id,
                "Zeynep",
                "Demir",
                "45678901234",
                new DateTime(1990, 7, 5),
                EnumGender.FEMALE,
                8,
                "Bursa",
                "Osmangazi",
                "zeynep.demir@example.com",
                "555-4567890"
            );

            var d6 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Yrd. Doç.").Id,
                departments[3].Id,
                "Ahmet",
                "Aksoy",
                "56789012345",
                new DateTime(1982, 11, 30),
                EnumGender.MALE,
                13,
                "Adana",
                "Seyhan",
                "ahmet.aksoy@example.com",
                "555-5678901"
            );

            var d7 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[2].Id,
                "Elif",
                "Çelik",
                "67890123456",
                new DateTime(1988, 4, 18),
                EnumGender.FEMALE,
                10,
                "Antalya",
                "Muratpaşa",
                "elif.celik@example.com",
                "555-6789012"
            );

            var d8 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[4].Id,
                "Mehmet",
                "Güneş",
                "78901234567",
                new DateTime(1970, 9, 9),
                EnumGender.MALE,
                25,
                "Gaziantep",
                "Şahinbey",
                "mehmet.gunes@example.com",
                "555-7890123"
            );

            var d9 = new Doctor(
                guidGenerator.Create(),
                titles.First(t => t.TitleName == "Yrd. Doç.").Id,
                departments[5].Id,
                "Ayşe",
                "Yıldız",
                "89012345678",
                new DateTime(1987, 6, 22),
                EnumGender.FEMALE,
                9,
                "Samsun",
                "Atakum",
                "ayse.yildiz@example.com",
                "555-8901234"
            );


            await doctorRepository.InsertAsync(d1, true);
            await doctorRepository.InsertAsync(d2, true);
            await doctorRepository.InsertAsync(d3, true);
            await doctorRepository.InsertAsync(d4, true);
            await doctorRepository.InsertAsync(d5, true);
            await doctorRepository.InsertAsync(d6, true);
            await doctorRepository.InsertAsync(d7, true);
            await doctorRepository.InsertAsync(d8, true);
            await doctorRepository.InsertAsync(d9, true);
        }

        private async Task SeedRoleRecords()
        {
            var doctor = new IdentityRole(guidGenerator.Create(), "doctor", null)
            {
                IsPublic = true,
                IsDefault = false
            };

            var staff = new IdentityRole(guidGenerator.Create(), "staff", null)
            {
                IsPublic = true,
                IsDefault = false
            };

            await roleManager.CreateAsync(doctor);
            await roleManager.CreateAsync(staff);

            //Patiens permissions
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Patients.Default, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Patients.Create, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Patients.Delete, true);

            //Departments permissions
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Departments.Default, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Departments.Create, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Departments.Delete, true);


            //Patiens permissions
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.Patients.Default, true);
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.Patients.Create, true);
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.Patients.Delete, true);

            //Departments permissions
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.MedicalServices.Default, true);
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.MedicalServices.Create, true);
            await permissionManager.SetForRoleAsync(staff.Name, HealthCarePermissions.MedicalServices.Delete, true);

            await userManager.CreateAsync(new IdentityUser(guidGenerator.Create(), "doctor@1", "doc1@gmail.com"),
                "doctor@A1");
        }

        private async Task SeedAppointments()
        {
            if (await medicalServiceRepository.GetCountAsync() == 0
                || await patientRepository.GetCountAsync() == 0
                || await doctorRepository.GetCountAsync() == 0)
                return;

            var medicalServices = await medicalServiceRepository.GetListAsync();
            var doctors = await doctorRepository.GetListAsync();
            var patients = await patientRepository.GetListAsync();

            var random = new Random();
            var startHour = TimeSpan.FromHours(9);
            var endHour = TimeSpan.FromHours(17);
            
            var allAppointments = new List<Appointment>();

            foreach (var patient in patients)
            {
                var doctor = doctors[random.Next(doctors.Count)]; 
                var medicalService = medicalServices[random.Next(medicalServices.Count)];
                var serviceDuration = TimeSpan.FromMinutes(medicalService.Duration);

                var appointmentDate = DateTime.Now.Date.AddDays(1);

                var availableTimes = new List<TimeSpan>();

                for (var time = startHour; time + serviceDuration <= endHour; time += serviceDuration)
                {
                    availableTimes.Add(time);
                }

                var patientAppointments =
                    new HashSet<(DateTime, TimeSpan)>();

                for (int i = 0; i < 10; i++)
                {
                    bool isSlotTaken;
                    TimeSpan appointmentTime;

                    do
                    {
                        appointmentTime = availableTimes[random.Next(availableTimes.Count)];
                        isSlotTaken = patientAppointments.Contains((appointmentDate, appointmentTime));
                    } while (isSlotTaken);

                    patientAppointments.Add((appointmentDate, appointmentTime));

                    try
                    {
                        var appointment = new Appointment(
                            guidGenerator.Create(),
                            doctor.Id,
                            patient.Id,
                            medicalService.Id,
                            appointmentDate,
                            appointmentDate + appointmentTime,
                            appointmentDate + appointmentTime + serviceDuration,
                            (EnumAppointmentStatus)random.Next(0, Enum.GetValues(typeof(EnumAppointmentStatus)).Length),
                            random.NextDouble() > 0.5 ? "Some notes for this appointment" : null,
                            random.NextDouble() > 0.5,
                            medicalService.Cost
                        );

                        allAppointments.Add(appointment);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(
                            $"Skipping appointment for patient {patient.Id} on {appointmentDate.Add(appointmentTime)}. Error: {ex.Message}");
                    }

                    // Her hasta için 10 farklı randevu saati olmalı
                    if (patientAppointments.Count >= 10)
                    {
                        break;
                    }

                    // Eğer tüm slotlar dolmuşsa bir sonraki gün başlayacak
                    if (appointmentTime + serviceDuration >= endHour)
                    {
                        appointmentDate = appointmentDate.AddDays(1); // Bir sonraki güne geçiş
                        availableTimes.Clear(); // O günün saat dilimlerini sıfırla
                        for (var time = startHour; time + serviceDuration <= endHour; time += serviceDuration)
                        {
                            availableTimes.Add(time); // Yeni güne ait saat dilimlerini oluştur
                        }
                    }
                }
            }

            await appointmentRepository.InsertManyAsync(allAppointments);
        }
        
        private async Task SeedDoctorWorkingHours()
        {
            if (await doctorRepository.GetCountAsync() == 0)
                return;

            var doctors = await doctorRepository.GetListAsync();

            foreach (var doctor in doctors)
            {
                for (var day = (int)DayOfWeek.Monday; day <= (int)DayOfWeek.Friday; day++)
                {
                    var startHour = new TimeSpan(8, 0, 0);
                    var endHour = new TimeSpan(17, 0, 0);

                    var doctorWorkingHour = new DoctorWorkingHour(doctor.Id, (DayOfWeek)day, startHour, endHour);
                    await doctorWorkingHourRepository.InsertAsync(doctorWorkingHour);
                }
            }
        }
    }
}