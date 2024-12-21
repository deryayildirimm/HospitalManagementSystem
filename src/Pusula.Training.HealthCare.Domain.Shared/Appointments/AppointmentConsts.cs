using System;

namespace Pusula.Training.HealthCare.Appointments;

public static class AppointmentConsts
{
    private const string DefaultSorting = "{0}AppointmentDate asc";
    public const EnumAppointmentGroupFilter DefaultGroupBy = EnumAppointmentGroupFilter.Department;
    public const string DefaultGroupBySorting = "{0}Number asc";
    
    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Appointment." : string.Empty);
    }
    
    public static string GetGroupDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultGroupBySorting, withEntityName ? "Appointment." : string.Empty);
    }
    
    public const double MinAmount = 0.0;
    public const double MaxAmount = 1000000000.0;
    
    public const int MinOffset = 0;
    public const int MaxOffset = 30;
    
    public const double StatusMinValue = 0;
    public const double StatusMaxValue = 3;

    public static readonly TimeSpan MinAppointmentTime = TimeSpan.FromHours(7);
    public static readonly TimeSpan MaxAppointmentTime = TimeSpan.FromHours(18);

    public const int MaxNotesLength = 1000;
    
    public static readonly EnumAppointmentStatus[] ValidStatuses = 
    {
        EnumAppointmentStatus.Scheduled,
        EnumAppointmentStatus.Completed,
        EnumAppointmentStatus.Cancelled,
        EnumAppointmentStatus.Missed
    };
}