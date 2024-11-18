using System;

namespace Pusula.Training.HealthCare.Appointments;

public class AppointmentConsts
{
    public const double MinAmount = 0.0;
    public const double MaxAmount = 1000000000.0;

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