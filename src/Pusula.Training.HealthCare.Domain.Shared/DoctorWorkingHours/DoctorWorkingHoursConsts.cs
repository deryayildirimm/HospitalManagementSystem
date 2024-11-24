using System;

namespace Pusula.Training.HealthCare.DoctorWorkingHours;

public class DoctorWorkingHoursConsts
{
    private const string DefaultSorting = "{0}DoctorId asc";
    
    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "DoctorWorkingHour." : string.Empty);
    }

    public static readonly TimeSpan MinStartHour = new TimeSpan(6, 0, 0);
    public static readonly TimeSpan MaxEndHour = new TimeSpan(22, 0, 0);
    
}