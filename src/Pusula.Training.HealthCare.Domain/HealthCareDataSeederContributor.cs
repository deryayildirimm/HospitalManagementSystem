using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.MedicalServices;
using Pusula.Training.HealthCare.Patients;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
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
        IMedicalServiceRepository medicalServiceRepository,
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
            await SeedDepartmentRecords();
            await SeedMedicalServiceRecords();
            await SeedMedicalServiceToDepartments();
            await SeedMedicalServicePatientRecords();
        }

        private async Task SeedMedicalServiceRecords()
        {
            await medicalServiceRepository.InsertAsync(
                new MedicalService(Guid.NewGuid(), "X-Ray", 300.00, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "MRI Scan", 1200.00,
                DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Cardiology Consultation",
                250.00, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Pediatric Check-up",
                120.00, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(
                new MedicalService(Guid.NewGuid(), "Chiropractic Session", 90.00, DateTime.Now));
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Nutrition Consultation",
                75.00, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Psychiatric Evaluation",
                300.0, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Radiology Review", 910.00,
                DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Physical Therapy Assessment",
                725.00, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Obstetrics Ultrasound",
                400.00, DateTime.Now), true);
            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Emergency Room Visit",
                500.00, DateTime.Now), true);

            await medicalServiceRepository.InsertAsync(new MedicalService(Guid.NewGuid(), "Examination",
                100.00, DateTime.Now), true);
        }

        private async Task SeedDepartmentRecords()
        {
            var departments = new List<Department>
            {
                new Department(Guid.NewGuid(), "Cardiology"),
                new Department(Guid.NewGuid(), "Radiology"),
                new Department(Guid.NewGuid(), "Emergency"),
                new Department(Guid.NewGuid(), "Pediatrics"),
                new Department(Guid.NewGuid(), "Oncology"),
                new Department(Guid.NewGuid(), "Neurology"),
                new Department(Guid.NewGuid(), "Orthopedics"),
                new Department(Guid.NewGuid(), "Dermatology"),
                new Department(Guid.NewGuid(), "Gastroenterology"),
                new Department(Guid.NewGuid(), "Urology"),
                new Department(Guid.NewGuid(), "Obstetrics and Gynecology"),
                new Department(Guid.NewGuid(), "Pulmonology"),
                new Department(Guid.NewGuid(), "Endocrinology"),
                new Department(Guid.NewGuid(), "Nephrology"),
                new Department(Guid.NewGuid(), "Psychiatry"),
                new Department(Guid.NewGuid(), "Hematology"),
                new Department(Guid.NewGuid(), "Ophthalmology"),
                new Department(Guid.NewGuid(), "Otolaryngology"),
                new Department(Guid.NewGuid(), "Anesthesiology"),
                new Department(Guid.NewGuid(), "Rheumatology"),
                new Department(Guid.NewGuid(), "Physical Therapy and Rehabilitation"),
                new Department(Guid.NewGuid(), "Pathology"),
                new Department(Guid.NewGuid(), "Allergy and Immunology"),
                new Department(Guid.NewGuid(), "Plastic Surgery"),
                new Department(Guid.NewGuid(), "General Surgery")
            };

            await departmentRepository.InsertManyAsync(departments, true);
        }

        private async Task SeedMedicalServiceToDepartments()
        {
            var medicalServices = await medicalServiceRepository.GetListAsync();
            var departments = await departmentRepository.GetListAsync();

            foreach (var department in departments)
            {
                var random = new Random();
                var randomServices = medicalServices.OrderBy(ms => random.Next()).Take(2).ToList();
                foreach (var service in randomServices)
                {
                    department.MedicalServices.Add(service);
                }
            }

            await departmentRepository.UpdateManyAsync(departments, true);
        }

        private async Task SeedPatientRecords()
        {
            var patient1 = new Patient(
                Guid.NewGuid(),
                "Ali",
                "Yılmaz",
                EnumNationality.TURKISH,
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
                "Mehmet",
                "Demir",
                EnumNationality.TURKISH,
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
                "Ayşe",
                "Kaya",
                EnumNationality.TURKISH,
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

        private async Task SeedMedicalServicePatientRecords()
        {
            var patients = await patientRepository.GetListAsync();
            var medicalServices = await medicalServiceRepository.GetListAsync();
            var random = new Random();

            foreach (var patient in patients)
            {
                var randomServices = medicalServices.OrderBy(ms => random.Next()).Take(medicalServices.Count).ToList();

                foreach (var medicalServicePatient in randomServices.Select(service => new MedicalServicePatient(
                             service.Id,
                             patient.Id,
                             service.Cost,
                             DateTime.Now
                         )))
                {
                    patient.MedicalServices.Add(medicalServicePatient);
                    await patientRepository.UpdateAsync(patient, true);
                }
            }
        }
    }
}