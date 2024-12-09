using System.Threading.Tasks;
using Pusula.Training.HealthCare.Localization;
using Pusula.Training.HealthCare.MultiTenancy;
using Pusula.Training.HealthCare.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace Pusula.Training.HealthCare.Blazor.Menus;

public class HealthCareMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<HealthCareResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                HealthCareMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 0
            )
        );

        ConfigureTenantMenu(administration, MultiTenancyConsts.IsEnabled);


        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.Patients,
                l["Menu:Patients"],
                url: "/patients",
                icon: "fa fa-users",
                requiredPermissionName: HealthCarePermissions.Patients.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.Protocols,
                l["Menu:Protocols"],
                url: "/protocols",
                icon: "fa fa-file-alt",
                requiredPermissionName: HealthCarePermissions.Protocols.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.Departments,
                l["Menu:Departments"],
                url: "/departments",
                icon: "fa fa-file-alt",
                requiredPermissionName: HealthCarePermissions.Departments.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.MedicalServices,
                l["Menu:MedicalServices"],
                url: "/medical-services",
                icon: "fa-solid fa-notes-medical",
                requiredPermissionName: HealthCarePermissions.MedicalServices.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.Doctors,
                l["Menu:Doctors"],
                url: "/doctors",
                icon: "fa fa-user-md",
                requiredPermissionName: HealthCarePermissions.Doctors.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.BloodTests,
                l["Menu:BloodTests"],
                url: "/bloodtest",
                icon: "fa-solid fa-droplet",
                requiredPermissionName: HealthCarePermissions.BloodTests.Edit)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.MyPatients,
                l["Menu:MyPatients"],
                url: "/doctor/my-patients",
                icon: "fa-solid fa-receipt",
                requiredPermissionName: HealthCarePermissions.Doctors.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.MedicalStaff,
                l["Menu:MedicalStaff"],
                url: "/medical-staff",
                icon: "fa fa-user-md",
                requiredPermissionName: HealthCarePermissions.MedicalStaff.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                HealthCareMenus.MedicalStaff,
                l["Menu:Insurances"],
                url: "/insurances",
                icon: "fa fa-user-md",
                requiredPermissionName: HealthCarePermissions.Insurances.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                    name: HealthCareMenus.Treatment,
                    displayName: l["Menu:Treatment"],
                    icon: "fa-solid fa-medkit"
                )
                .AddItem(
                    new ApplicationMenuItem(
                            name: "Definitions",
                            displayName: l["Menu:Definitions"],
                            icon: "fa-solid fa-folder"
                        )
                        .AddItem(new ApplicationMenuItem(
                            name: HealthCareMenus.Icds,
                            displayName: l["Menu:Icds"],
                            url: "/icds",
                            icon: "fa-solid fa-plus-square")
                        )
                )
                .AddItem(
                    new ApplicationMenuItem(
                            name: "Operations",
                            displayName: l["Menu:Operations"],
                            icon: "fa-solid fa-cogs"
                        )
                    
                )
                .AddItem(
                    new ApplicationMenuItem(
                            name: "Reports",
                            displayName: l["Menu:Reports"],
                            icon: "fa-solid fa-chart-bar"
                        )
                        .AddItem(new ApplicationMenuItem(
                            name: HealthCareMenus.IcdReport,
                            displayName: l["Menu:IcdReport"],
                            url: "/icd-report",
                            icon: "fa-solid fa-plus-square")
                        )
                ));
        return Task.CompletedTask;
    }

    private static void ConfigureTenantMenu(ApplicationMenuItem? item, bool isMultiTenancyEnabled)
    {
        if (isMultiTenancyEnabled)
        {
            item?.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            item?.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }
    }
}

