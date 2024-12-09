using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Appointments;
using Pusula.Training.HealthCare.Cities;
using Pusula.Training.HealthCare.BloodTests.Categories;
using Pusula.Training.HealthCare.BloodTests.Tests;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Districts;
using Pusula.Training.HealthCare.Doctors;
using Pusula.Training.HealthCare.DoctorWorkingHours;
using Pusula.Training.HealthCare.Insurances;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Protocols;
using Pusula.Training.HealthCare.ProtocolTypes;
using Pusula.Training.HealthCare.Titles;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Pusula.Training.HealthCare.Insurances;

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
        ICityRepository cityRepository,
        IDistrictRepository districtRepository,
        IGuidGenerator guidGenerator,
        ITestCategoryRepository testCategoryRepository,
        IProtocolTypeRepository protocolTypeRepository,
        IProtocolRepository protocolRepository,
        IInsuranceRepository insuranceRepository,
        IRepository<Test, Guid> testRepository) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {
            await SetRoles();
        }

        private async Task SetRoles()
        {
            await SeedCityRecords();
           await SeedPatientRecords();
            await SeedRoleRecords();
           await SeedDistrictRecords();
            await SeedMedicalServiceRecords();
            await SeedDepartmentRecords();
           await SeedMedicalServiceToDepartments();
            await SeedTitles();
            await SeedDoctorRecords();
            await SeedDoctorWorkingHours();
           await SeedAppointments();
          await SeedTestCategoryRecords();
          await SeedTestRecords(); 
          await SeedProtocolType();
        //  await SeedInsurance();
            await SeedProtocols(); 
            await SeedProtocolMedicalService();

        }

        private async Task SeedProtocolType()
        {
            var types = new List<ProtocolType>
            {
                new ProtocolType(guidGenerator.Create(), "Ayakta"),
                new ProtocolType(guidGenerator.Create(), "Yatış"),
                new ProtocolType(guidGenerator.Create(), "Kontrol"),
            
            };

            await protocolTypeRepository.InsertManyAsync(types, autoSave: true);
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
        private async Task SeedMedicalServiceRecords()
        {
            if (await medicalServiceRepository.GetCountAsync() > 0)
            {
                return;
            }

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
            {
                return;
            }

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
            {
                return;
            }

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

        
        private async Task SeedProtocolMedicalService()
        {
            if (await medicalServiceRepository.GetCountAsync() == 0
                || await protocolRepository.GetCountAsync() == 0)
            {
                return;
            }

            var medicalServices = await medicalServiceRepository.GetListAsync();
            var protocols = await protocolRepository.GetListAsync();

            foreach (var protocol in protocols)
            {
                var random = new Random();

                var randomServices = medicalServices.OrderBy(ms => random.Next()).Take(2).ToList();
                foreach (var service in randomServices)
                {
                    var protocolMedicalService = new ProtocolMedicalService
                    {
                        MedicalServiceId = service.Id,
                        ProtocolId = protocol.Id
                    };

                    service.ProtocolMedicalServices.Add(protocolMedicalService);
                }
            }

            await protocolRepository.UpdateManyAsync(protocols, true);
        }

        private async Task SeedPatientRecords()
        {
            if (await patientRepository.GetCountAsync() > 0)
            {
                return;
            }

            var patient1 = new Patient(
                Guid.NewGuid(),
                1,
                "Hasan",
                "Kuru",
                EnumGender.MALE,
                new DateTime(1990, 1, 1),
                "A12345678",
                "Turkey",
                "+12345678901",
                EnumPatientTypes.VIP,
                "Fatma",
                "Aykut",
                "ali.yilmaz@example.com",
                EnumRelative.FATHER,
                "+12344678901",
                "İstanbul",
                EnumDiscountGroup.STAFF
            );

            var patient2 = new Patient(
                Guid.NewGuid(),
                2,
                "Mert",
                "Demir",
                EnumGender.MALE,
                new DateTime(1985, 5, 10),
                "B98765432",
                "Ireland",
                "+98765432109",
                EnumPatientTypes.NORMAL,
                "Emine",
                "Suat",
                "mehmet.demir@example.com",
                EnumRelative.MOTHER,
                "+09876543210",
                "Ankara",
                EnumDiscountGroup.STAFF
            );

            var patient3 = new Patient(
                Guid.NewGuid(),
                3,
                "Leyla",
                "Kaya",
                EnumGender.FEMALE,
                new DateTime(1992, 3, 15),
                "C12312312",
                "Germany",
                "+12312312345",
                EnumPatientTypes.VIP,
                "Huriye",
                "Aslan",
                "ayse.kaya@example.com",
                EnumRelative.FATHER,
                "+11223344556",
                "Izmir",
                EnumDiscountGroup.CONTRACTED
            );

            await patientRepository.InsertAsync(patient1,true);
            await patientRepository.InsertAsync(patient2,true);
            await patientRepository.InsertAsync(patient3,true);
        }

        private async Task SeedTestCategoryRecords()
        {
            if (await testCategoryRepository.GetCountAsync() > 0)
                return;

            await testCategoryRepository.InsertAsync(
                new TestCategory(Guid.NewGuid(), "Hematological Tests", "Measures blood cells (red blood cells, white blood cells) and related values.", "1.png", 1500), true);
            await testCategoryRepository.InsertAsync(
                new TestCategory(Guid.NewGuid(), "Biochemical Tests", "Measures vitamins, mineral levels and other chemical values.", "2.jpg", 2000), true);
            await testCategoryRepository.InsertAsync(
                new TestCategory(Guid.NewGuid(), "Hormonal Tests", "Measures hormone levels and endocrine functions.", "3.jpg", 2500), true);
        }

        private async Task SeedTestRecords()
        {
            var testCount = await testRepository.CountAsync();
            if (testCount > 0)
                return;

            var hematologicalCategory = await testCategoryRepository.FirstOrDefaultAsync(tc => tc.Name == "Hematological Tests");
            var biochemicalCategory = await testCategoryRepository.FirstOrDefaultAsync(tc => tc.Name == "Biochemical Tests");
            var hormonalCategory = await testCategoryRepository.FirstOrDefaultAsync(tc => tc.Name == "Hormonal Tests");

            if (hematologicalCategory == null || biochemicalCategory == null || hormonalCategory == null)
            {
                throw new Exception("Test categories not found.");
            }

            var testList = new List<Test>
            {
                new Test(guidGenerator.Create(),hematologicalCategory.Id,"Hemoglobin",13.5,17.5),
                new Test(guidGenerator.Create(),hematologicalCategory.Id,"Hematocrit",38.0,50.0),
                new Test(guidGenerator.Create(),hematologicalCategory.Id,"White Blood Cell Count",4.0,11.0),

                new Test(guidGenerator.Create(),biochemicalCategory.Id,"Glucose",70,99),
                new Test(guidGenerator.Create(),biochemicalCategory.Id,"Cholesterol",125,200),
                new Test(guidGenerator.Create(),biochemicalCategory.Id,"Creatinine",0.6,1.2),

                new Test(guidGenerator.Create(),hormonalCategory.Id,"TSH",0.5,5.5),
                new Test(guidGenerator.Create(),hormonalCategory.Id,"Free T3",2.3,4.2),
                new Test(guidGenerator.Create(),hormonalCategory.Id,"Free T4",0.7,1.8)
            };
            await testRepository.InsertManyAsync(testList);
        }

        private async Task SeedCityRecords()
        {
            if (await cityRepository.GetCountAsync() > 0)
            {
                return;
            }

            var c1 = new City(guidGenerator.Create(), "Istanbul");
            var c2 = new City(guidGenerator.Create(), "Ankara");
            var c3 = new City(guidGenerator.Create(), "İzmir");
            var c4 = new City(guidGenerator.Create(), "Antalya");
            var c5 = new City(guidGenerator.Create(), "Bursa");

            await cityRepository.InsertManyAsync([c1, c2, c3, c4, c5], true);
        }

        private async Task SeedDistrictRecords()
        {
            if (await districtRepository.GetCountAsync() > 0)
                return;

            var cityDistricts = new Dictionary<string, List<string>>
            {
                { "Istanbul", ["Kadıköy", "Üsküdar", "Pendik", "Bakırköy", "Sarıyer"] },
                { "Ankara", ["Çankaya", "Keçiören", "Yenimahalle", "Mamak", "Altındağ"] },
                { "Izmir", ["Konak", "Bornova", "Karşıyaka", "Buca", "Gaziemir"] },
                { "Bursa", ["Nilüfer", "Osmangazi", "Yıldırım", "Gemlik", "İnegöl"] },
                { "Antalya", ["Muratpaşa", "Kepez", "Konyaaltı", "Alanya", "Manavgat"] }
            };

            var cities = await cityRepository.GetListAsync();

            foreach (var city in cities)
            {
                if (!cityDistricts.TryGetValue(city.Name, out var districts))
                {
                    continue;
                }

                foreach (var district in districts.Select(districtName =>
                             new District(guidGenerator.Create(), city.Id, districtName)))
                {
                    await districtRepository.InsertAsync(district);
                }
            }
        }

        private async Task SeedDoctorRecords()
        {
            if (await titleRepository.GetCountAsync() == 0
                || await departmentRepository.GetCountAsync() == 0
                || await cityRepository.GetCountAsync() == 0
                || await districtRepository.GetCountAsync() == 0)
                return;

            var titles = await titleRepository.GetListAsync();
            var departments = await departmentRepository.GetListAsync();
            var cityTitles = await cityRepository.GetListAsync();
            var districtTitles = await districtRepository.GetListWithNavigationPropertiesAsync();

            var city = cityTitles.First(x => x.Name == "Istanbul");
            var district = districtTitles.First(d => d.City.Name == "Istanbul");

            var d1 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Arif",
                "Yılmaz",
                "12345678901",
                new DateTime(1980, 5, 12),
                EnumGender.MALE,
                new DateTime(1999, 5, 12),
                "ahmet.yilmaz@example.com",
                "555-1234567"
            );

            var d2 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Fatma",
                "Kara",
                "98765432109",
                new DateTime(1990, 3, 25),
                EnumGender.FEMALE,
                new DateTime(2001, 5, 12),
                "fatma.kara@example.com",
                "555-9876543"
            );

            var d3 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[0].Id,
                "Mehmet",
                "Çelik",
                "12309876543",
                new DateTime(1975, 11, 30),
                EnumGender.MALE,
                new DateTime(2005, 11, 30),
                "mehmet.celik@example.com",
                "555-3219876"
            );

            var d4 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[1].Id,
                "Merve",
                "Şahin",
                "23456789012",
                new DateTime(1985, 8, 23),
                EnumGender.FEMALE,
                new DateTime(2005, 11, 30),
                "merve.sahin@example.com",
                "555-2345678"
            );

            var d5 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[1].Id,
                "Zeynep",
                "Demir",
                "45678901234",
                new DateTime(1990, 7, 5),
                EnumGender.FEMALE,
                new DateTime(2009, 7, 5),
                "zeynep.demir@example.com",
                "555-4567890"
            );

            var d6 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Yrd. Doç.").Id,
                departments[3].Id,
                "Ahmet",
                "Aksoy",
                "56789012345",
                new DateTime(1982, 11, 30),
                EnumGender.MALE,
                new DateTime(2012, 7, 5),
                "ahmet.aksoy@example.com",
                "555-5678901"
            );

            var d7 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Dr.").Id,
                departments[2].Id,
                "Elif",
                "Çelik",
                "67890123456",
                new DateTime(1988, 4, 18),
                EnumGender.FEMALE,
                new DateTime(2017, 4, 18),
                "elif.celik@example.com",
                "555-6789012"
            );

            var d8 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Prof.").Id,
                departments[4].Id,
                "Mehmet",
                "Güneş",
                "78901234567",
                new DateTime(1970, 9, 9),
                EnumGender.MALE,
                new DateTime(2019, 9, 9),
                "mehmet.gunes@example.com",
                "555-7890123"
            );

            var d9 = new Doctor(
                guidGenerator.Create(),
                city.Id,
                district.District.Id,
                titles.First(t => t.TitleName == "Yrd. Doç.").Id,
                departments[5].Id,
                "Ayşe",
                "Yıldız",
                "89012345678",
                new DateTime(1987, 6, 22),
                EnumGender.FEMALE,
                new DateTime(2023, 6, 22),
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
        
        
        
        private async Task SeedProtocols()
        {
            // Doktorlar, hastalar, protokol türleri ve departmanlar kontrol ediliyor
            var doctors = await doctorRepository.GetListAsync();
            var patients = await patientRepository.GetListAsync();
            var protocolTypes = await protocolTypeRepository.GetListAsync();
            var departments = await departmentRepository.GetListAsync();

    if (!doctors.Any() || !patients.Any() || !protocolTypes.Any() || !departments.Any())
    {
        throw new Exception("Doctors, Patients, ProtocolTypes, or Departments are missing. Seed them first.");
    }

    var random = new Random();
    var protocols = new List<Protocol>();
    var insurances = new List<Insurance>();

    foreach (var patient in patients)
    {
        var doctor = doctors[random.Next(doctors.Count)];
        var department = departments[random.Next(departments.Count)];
        var protocolType = protocolTypes[random.Next(protocolTypes.Count)];
        var startTime = DateTime.Now.AddDays(random.Next(1, 10));
        var endTime = startTime.AddHours(1);

        // Protokole özel bir sigorta oluşturuluyor
        var insurance = new Insurance(
            id: Guid.NewGuid(),
            policyNumber: $"POL-{random.Next(1000, 9999)}",
            (EnumInsuranceCompanyName)random.Next(1, 10),
            premiumAmount: random.Next(100, 1000),
            coverageAmount: random.Next(1000, 10000),
            startDate: DateTime.UtcNow.AddDays(-30),
            endDate: DateTime.UtcNow.AddYears(1),
            description: "Insurance for protocol"
        );

        insurances.Add(insurance); // Sigortayı listeye ekliyoruz

        protocols.Add(new Protocol(
            id: Guid.NewGuid(),
            patientId: patient.Id,
            departmentId: department.Id,
            doctorId: doctor.Id,
            protocolTypeId: protocolType.Id,
            startTime: startTime,
            note: "Routine checkup",
            endTime: endTime,
            insuranceId: insurance.Id // Sigorta ID'si atanıyor
        ));
    }

    await insuranceRepository.InsertManyAsync(insurances, autoSave: true); // Sigortalar veri tabanına ekleniyor
    await protocolRepository.InsertManyAsync(protocols, autoSave: true); // Protokoller veri tabanına ekleniyor
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

            //Doctor permissions
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Doctors.Default, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Doctors.Create, true);
            await permissionManager.SetForRoleAsync(doctor.Name, HealthCarePermissions.Doctors.Delete, true);

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