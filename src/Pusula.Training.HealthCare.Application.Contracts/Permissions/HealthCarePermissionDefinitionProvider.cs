using Pusula.Training.HealthCare.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Pusula.Training.HealthCare.Permissions;

public class HealthCarePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(HealthCarePermissions.GroupName);

        myGroup.AddPermission(HealthCarePermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(HealthCarePermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(HealthCarePermissions.MyPermission1, L("Permission:MyPermission1"));

        var patientPermission = myGroup.AddPermission(HealthCarePermissions.Patients.Default, L("Permission:Patients"));
        patientPermission.AddChild(HealthCarePermissions.Patients.Create, L("Permission:Create"));
        patientPermission.AddChild(HealthCarePermissions.Patients.Edit, L("Permission:Edit"));
        patientPermission.AddChild(HealthCarePermissions.Patients.Delete, L("Permission:Delete"));

        var protocolPermission = myGroup.AddPermission(HealthCarePermissions.Protocols.Default, L("Permission:Protocols"));
        protocolPermission.AddChild(HealthCarePermissions.Protocols.Create, L("Permission:Create"));
        protocolPermission.AddChild(HealthCarePermissions.Protocols.Edit, L("Permission:Edit"));
        protocolPermission.AddChild(HealthCarePermissions.Protocols.Delete, L("Permission:Delete"));
        
        var protocolTypePermission = myGroup.AddPermission(HealthCarePermissions.ProtocolTypes.Default, L("Permission:ProtocolTypes"));
        protocolTypePermission.AddChild(HealthCarePermissions.ProtocolTypes.Create, L("Permission:Create"));
        protocolTypePermission.AddChild(HealthCarePermissions.ProtocolTypes.Edit, L("Permission:Edit"));
        protocolTypePermission.AddChild(HealthCarePermissions.ProtocolTypes.Delete, L("Permission:Delete"));

        var departmentPermission = myGroup.AddPermission(HealthCarePermissions.Departments.Default, L("Permission:Departments"));
        departmentPermission.AddChild(HealthCarePermissions.Departments.Create, L("Permission:Create"));
        departmentPermission.AddChild(HealthCarePermissions.Departments.Edit, L("Permission:Edit"));
        departmentPermission.AddChild(HealthCarePermissions.Departments.Delete, L("Permission:Delete"));
        
        var medicalServicesPermission = myGroup.AddPermission(HealthCarePermissions.MedicalServices.Default, L("Permission:MedicalServices"));
        medicalServicesPermission.AddChild(HealthCarePermissions.MedicalServices.Create, L("Permission:Create"));
        medicalServicesPermission.AddChild(HealthCarePermissions.MedicalServices.Edit, L("Permission:Edit"));
        medicalServicesPermission.AddChild(HealthCarePermissions.MedicalServices.Delete, L("Permission:Delete"));
        
        var appointmentPermission = myGroup.AddPermission(HealthCarePermissions.Appointments.Default, L("Permission:Appointments"));
        appointmentPermission.AddChild(HealthCarePermissions.Appointments.Create, L("Permission:Create"));
        appointmentPermission.AddChild(HealthCarePermissions.Appointments.Edit, L("Permission:Edit"));
        appointmentPermission.AddChild(HealthCarePermissions.Appointments.Delete, L("Permission:Delete"));

        var titlePermission = myGroup.AddPermission(HealthCarePermissions.Titles.Default, L("Permission:Titles"));
        titlePermission.AddChild(HealthCarePermissions.Titles.Create, L("Permission:Create"));
        titlePermission.AddChild(HealthCarePermissions.Titles.Edit, L("Permission:Edit"));
        titlePermission.AddChild(HealthCarePermissions.Titles.Delete, L("Permission:Delete"));

        var doctorPermission = myGroup.AddPermission(HealthCarePermissions.Doctors.Default, L("Permission:Doctors"));
        doctorPermission.AddChild(HealthCarePermissions.Doctors.Create, L("Permission:Create"));
        doctorPermission.AddChild(HealthCarePermissions.Doctors.Edit, L("Permission:Edit"));
        doctorPermission.AddChild(HealthCarePermissions.Doctors.Delete, L("Permission:Delete"));
        
        var doctorLeavePermission = myGroup.AddPermission(HealthCarePermissions.DoctorLeaves.Default, L("Permission:DoctorLeaves"));
        doctorLeavePermission.AddChild(HealthCarePermissions.DoctorLeaves.Create, L("Permission:Create"));
        doctorLeavePermission.AddChild(HealthCarePermissions.DoctorLeaves.Edit, L("Permission:Edit"));
        doctorLeavePermission.AddChild(HealthCarePermissions.DoctorLeaves.Delete, L("Permission:Delete"));
        
        var medicalStaffPermission = myGroup.AddPermission(HealthCarePermissions.MedicalStaff.Default, L("Permission:MedicalStaff"));
        medicalStaffPermission.AddChild(HealthCarePermissions.MedicalStaff.Create, L("Permission:Create"));
        medicalStaffPermission.AddChild(HealthCarePermissions.MedicalStaff.Edit, L("Permission:Edit"));
        medicalStaffPermission.AddChild(HealthCarePermissions.MedicalStaff.Delete, L("Permission:Delete"));

        var cityPermission = myGroup.AddPermission(HealthCarePermissions.Cities.Default, L("Permission:Cities"));
        cityPermission.AddChild(HealthCarePermissions.Cities.Create, L("Permission:Create"));
        cityPermission.AddChild(HealthCarePermissions.Cities.Edit, L("Permission:Edit"));
        cityPermission.AddChild(HealthCarePermissions.Cities.Delete, L("Permission:Delete"));

        var districtPermission = myGroup.AddPermission(HealthCarePermissions.Districts.Default, L("Permission:Districts"));
        districtPermission.AddChild(HealthCarePermissions.Districts.Create, L("Permission:Create"));
        districtPermission.AddChild(HealthCarePermissions.Districts.Edit, L("Permission:Edit"));
        districtPermission.AddChild(HealthCarePermissions.Districts.Delete, L("Permission:Delete"));

        var bloodTestPermission = myGroup.AddPermission(HealthCarePermissions.BloodTests.Default, L("Permission:BloodTests"));
        bloodTestPermission.AddChild(HealthCarePermissions.BloodTests.Create, L("Permission:Create"));
        bloodTestPermission.AddChild(HealthCarePermissions.BloodTests.Edit, L("Permission:Edit"));
        bloodTestPermission.AddChild(HealthCarePermissions.BloodTests.Delete, L("Permission:Delete"));
        
        var testCategoriesPermission = myGroup.AddPermission(HealthCarePermissions.TestCategories.Default, L("Permission:TestCategories"));
        testCategoriesPermission.AddChild(HealthCarePermissions.TestCategories.Create, L("Permission:Create"));
        testCategoriesPermission.AddChild(HealthCarePermissions.TestCategories.Edit, L("Permission:Edit"));
        testCategoriesPermission.AddChild(HealthCarePermissions.TestCategories.Delete, L("Permission:Delete"));

        var bloodTestResultPermission = myGroup.AddPermission(HealthCarePermissions.BloodTestResults.Default, L("Permission:BloodTestResults"));
        bloodTestResultPermission.AddChild(HealthCarePermissions.BloodTestResults.Create, L("Permission:Create"));
        bloodTestResultPermission.AddChild(HealthCarePermissions.BloodTestResults.Edit, L("Permission:Edit"));
        bloodTestResultPermission.AddChild(HealthCarePermissions.BloodTestResults.Delete, L("Permission:Delete"));

        var icdPermission = myGroup.AddPermission(HealthCarePermissions.Icds.Default, L("Permission:Icds"));
        icdPermission.AddChild(HealthCarePermissions.Icds.Create, L("Permission:Create"));
        icdPermission.AddChild(HealthCarePermissions.Icds.Edit, L("Permission:Edit"));
        icdPermission.AddChild(HealthCarePermissions.Icds.Delete, L("Permission:Delete"));

        var insurancePermission = myGroup.AddPermission(HealthCarePermissions.Insurances.Default, L("Permission:Insurances"));
        insurancePermission.AddChild(HealthCarePermissions.Insurances.Create, L("Permission:Create"));
        insurancePermission.AddChild(HealthCarePermissions.Insurances.Edit, L("Permission:Edit"));
        insurancePermission.AddChild(HealthCarePermissions.Insurances.Delete, L("Permission:Delete"));

        var familyHistoryPermission = myGroup.AddPermission(HealthCarePermissions.FamilyHistories.Default, L("Permission:FamilyHistories"));
        familyHistoryPermission.AddChild(HealthCarePermissions.FamilyHistories.Create, L("Permission:Create"));
        familyHistoryPermission.AddChild(HealthCarePermissions.FamilyHistories.Edit, L("Permission:Edit"));
        familyHistoryPermission.AddChild(HealthCarePermissions.FamilyHistories.Delete, L("Permission:Delete"));
        
        var backgroundPermission = myGroup.AddPermission(HealthCarePermissions.Backgrounds.Default, L("Permission:Backgrounds"));
        backgroundPermission.AddChild(HealthCarePermissions.Backgrounds.Create, L("Permission:Create"));
        backgroundPermission.AddChild(HealthCarePermissions.Backgrounds.Edit, L("Permission:Edit"));
        backgroundPermission.AddChild(HealthCarePermissions.Backgrounds.Delete, L("Permission:Delete"));
        
        var examinationPermission = myGroup.AddPermission(HealthCarePermissions.Examinations.Default, L("Permission:Examinations"));
        examinationPermission.AddChild(HealthCarePermissions.Examinations.Create, L("Permission:Create"));
        examinationPermission.AddChild(HealthCarePermissions.Examinations.Edit, L("Permission:Edit"));
        examinationPermission.AddChild(HealthCarePermissions.Examinations.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HealthCareResource>(name);
    }
}