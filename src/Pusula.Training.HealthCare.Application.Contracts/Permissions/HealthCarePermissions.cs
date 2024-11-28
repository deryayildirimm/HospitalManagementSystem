namespace Pusula.Training.HealthCare.Permissions;

public static class HealthCarePermissions
{
    public const string GroupName = "HealthCare";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Patients
    {
        public const string Default = GroupName + ".Patients";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Protocols
    {
        public const string Default = GroupName + ".Protocols";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Titles
    {
        public const string Default = GroupName + ".Titles";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Doctors
    {
        public const string Default = GroupName + ".Doctors";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class MedicalServices
    {
        public const string Default = GroupName + ".MedicalServices";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class Appointments
    {
        public const string Default = GroupName + ".Appointments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class AppointmentTypes
    {
        public const string Default = GroupName + ".AppointmentTypes";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class DoctorLeaves
    {
        public const string Default = GroupName + ".DoctorLeaves";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class MedicalStaff
    {
        public const string Default = GroupName + ".MedicalStaff";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class Cities
    {
        public const string Default = GroupName + ".Cities";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    
    public static class Districts
    {
        public const string Default = GroupName + ".Districts";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class BloodTests
    {
        public const string Default = GroupName + ".BloodTests";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class TestCategories
    {
        public const string Default = GroupName + ".TestCategories";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    public static class BloodTestResults
    {
        public const string Default = GroupName + ".BloodTestResults";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}