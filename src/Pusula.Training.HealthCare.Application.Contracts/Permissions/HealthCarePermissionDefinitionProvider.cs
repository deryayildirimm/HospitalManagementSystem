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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HealthCareResource>(name);
    }
}