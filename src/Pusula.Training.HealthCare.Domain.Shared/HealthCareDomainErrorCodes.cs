namespace Pusula.Training.HealthCare;

public static class HealthCareDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    
    public const string DoctorNotFound = "HealthCareError:DoctorNotFound";
    public const string DoctorWorkingHourNotFound = "HealthCareError:DoctorWorkingHourNotFound";
    
    public const string MedicalServiceNotFound = "HealthCareError:MedicalServiceNotFound";
    
    public const string NameExists = "HealthCareError:NameAlreadyExist";
    
    public const string AppointmentAlreadyTaken = "HealthCareError:AppointmentAlreadyTaken";
    public const string AppointmentDateNotValid = "HealthCareError:AppointmentDateNotValid";
    
    public const string InvalidDepartments = "HealthCareError:InvalidDepartments";
    public const string DepartmentsNotFound = "HealthCareError:DepartmentsNotFound";

    public const string DateNotInPastException = "HealthCareError:DateNotInPast";

    
}
