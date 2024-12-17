using System.Collections.Generic;

namespace Pusula.Training.HealthCare;

public static class HealthCareDomainErrorKeyValuePairs
{
    /* You can add your business exception error message here, as constants */

    public static KeyValuePair<string, string> GroupNameNotValid =
        KeyValuePair.Create(HealthCareDomainErrorCodes.NameExists, "Group name not valid.");

    public static KeyValuePair<string, string> NameAlreadyExists =
        KeyValuePair.Create(HealthCareDomainErrorCodes.NameExists, "You have already a record with this name.");

    public static KeyValuePair<string, string> DateNotValid =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DateNotInPastException, "Not valid date.");

    public static KeyValuePair<string, string> GenderNotValid =
        KeyValuePair.Create(HealthCareDomainErrorCodes.GenderNotValid, "Gender not valid.");

    public static KeyValuePair<string, string> DoctorRestrictionAlreadyExists =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DoctorRestrictionAlreadyExists,
            "Doctor's restriction already exists.");

    public static KeyValuePair<string, string> RestrictionAlreadyExists =
        KeyValuePair.Create(HealthCareDomainErrorCodes.RestrictionAlreadyExists, "Restriction already exists.");

    public static KeyValuePair<string, string> ValueExceedLimit =
        KeyValuePair.Create(HealthCareDomainErrorCodes.ValueExceedLimit, "Value exceed limit.");

    public static KeyValuePair<string, string> GenderNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.GenderNotValid, "Gender not valid.");

    public static KeyValuePair<string, string> TextLenghtExceeded =
        KeyValuePair.Create(HealthCareDomainErrorCodes.TextLenghtExceeded,
            "Text lenght exceeded. Max lenght is 1000 characters.");

    public static KeyValuePair<string, string> AlreadyHaveAppointmentExactTime =
        KeyValuePair.Create(HealthCareDomainErrorCodes.AlreadyHaveAppointmentExactTime,
            "Patient already have an appointment at this time.");

    public static KeyValuePair<string, string> AppointmentAlreadyTaken =
        KeyValuePair.Create(HealthCareDomainErrorCodes.AppointmentAlreadyTaken, "Selected appointment already taken.");
    
    public static KeyValuePair<string, string> AppointmentNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.AppointmentNotFound, "Appointment not found.");

    public static KeyValuePair<string, string> DoctorHasLeave =
        KeyValuePair.Create(HealthCareDomainErrorCodes.AppointmentAlreadyTaken,
            "Selected doctor has leave at these dates.");

    public static KeyValuePair<string, string> DoctorNotWorking =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DoctorNotWorking, "Doctor doesnt work at these times.");

    public static KeyValuePair<string, string> DoctorWorkingHourNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DoctorWorkingHourNotFound, "Doctor doesnt work at these dates.");

    public static KeyValuePair<string, string> MedicalServiceNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.MedicalServiceNotFound, "MedicalService not found.");

    public static KeyValuePair<string, string> RestrictionNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.RestrictionNotFound, "Restriction not found.");

    public static KeyValuePair<string, string> DepartmentNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.DepartmentsNotFound, "Department not found.");

    public static KeyValuePair<string, string> PatientAlreadyExist =
        KeyValuePair.Create(HealthCareDomainErrorCodes.PatientAlreadyExist,
            "A patient with the same Identity Number already exists.");

    public static KeyValuePair<string, string> InvalidDownloadToken =
        KeyValuePair.Create(HealthCareDomainErrorCodes.InvalidDownloadToken_CODE,
            "The provided download token is invalid.");

    public static KeyValuePair<string, string> PatientNotFound =
        KeyValuePair.Create(HealthCareDomainErrorCodes.PatientNotFound, "Patient not found.");
    
    public static KeyValuePair<string, string> ServiceAgeRestriction =
        KeyValuePair.Create(HealthCareDomainErrorCodes.PatientNotFound, "Patient age restriction is invalid this medical service.");
    
    public static KeyValuePair<string, string> ServiceGenderRestrictionViolation =
        KeyValuePair.Create(HealthCareDomainErrorCodes.PatientNotFound, "Gender restriction is invalid for this medical service.");
    
}