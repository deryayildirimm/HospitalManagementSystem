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
            await SeedMedicalServiceRecords();
            await SeedDepartmentRecords();
            await SeedMedicalServiceToDepartments();
        }

        private async Task SeedMedicalServiceRecords()
        {
            if (await medicalServiceRepository.GetCountAsync() > 0)
                return;
            
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
        
    }
}