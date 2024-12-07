using System.Collections.Generic;

namespace Pusula.Training.HealthCare;

public static class HealthCareDomainErrorKeyValuePairs
{
    /* You can add your business exception error message here, as constants */

    public static KeyValuePair<string, string> NameAlreadyExists =
        KeyValuePair.Create(HealthCareDomainErrorCodes.NameExists, "You have already a record with this name.");

    public static KeyValuePair<string, string> DateNotValid =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DateNotInPastException, "Not valid date.");

    public static KeyValuePair<string, string> AppointmentAlreadyTaken =
        KeyValuePair.Create(HealthCareDomainErrorCodes.AppointmentAlreadyTaken, "Selected appointment already taken.");

    public static KeyValuePair<string, string> DoctorNotWorking =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DoctorNotWorking, "Doctor doesnt work at these times.");

    public static KeyValuePair<string, string> DoctorWorkingHourNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DoctorWorkingHourNotFound, "Doctor doesnt work at these dates.");

    public static KeyValuePair<string, string> MedicalServiceNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.MedicalServiceNotFound, "MedicalService not found.");

    public static KeyValuePair<string, string> DepartmentNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DepartmentsNotFound, "Department not found.");
}