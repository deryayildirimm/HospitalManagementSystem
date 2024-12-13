namespace Pusula.Training.HealthCare;

public static class HealthCareDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    
    public const string DoctorNotFound = "HealthCareError:DoctorNotFound";
    public const string DoctorWorkingHourNotFound = "HealthCareError:DoctorWorkingHourNotFound";
    public const string DoctorNotWorking = "HealthCareError:DoctorNotWorking";
    public const string MedicalServiceNotFound = "HealthCareError:MedicalServiceNotFound";
    
    public const string NameExists = "HealthCareError:NameAlreadyExist";
    public const string GroupNameNotValid = "HealthCareError:GroupNameNotValid";
    
    public const string TextLenghtExceeded = "HealthCareError:TextLenghtExceeded";
    public const string GenderNotValid = "HealthCareError:GenderNotValid";
    public const string ValueExceedLimit = "HealthCareError:ValueExceedLimit";
    
    
    public const string AppointmentAlreadyTaken = "HealthCareError:AppointmentAlreadyTaken";
    public const string AppointmentDateNotValid = "HealthCareError:AppointmentDateNotValid";
    public const string AlreadyHaveAppointmentExactTime = "HealthCareError:AlreadyHaveAppointmentExactTime";
    
    public const string RestrictionNotFound = "HealthCareError:RestrictionNotFound";

    
    public const string InvalidDepartments = "HealthCareError:InvalidDepartments";
    public const string DepartmentsNotFound = "HealthCareError:DepartmentsNotFound";

    public const string DateNotInPastException = "HealthCareError:DateNotInPast";
    
   
    /*  ERROR MESSAGES */
    public const string InvalidDateRange_MESSAGE = "InvalidDateRange";
    public const string InvalidDownloadToken_MESSAGE = "The provided download token is invalid.";
    public const string HealthcareError_MESSAGE = "Healthcare error occured!!";
    public const string InvalidNoteLength_MESSAGE = "Notes must be between {1} and {100} characters.";
    public const string ProtocolUpdate_MESSAGE = "Protocol not found with the given ID.";
    
    /*  CODES */
    public const string ValidationError_CODE = "400";          
    public const string Forbidden_CODE = "403";               
    public const string InternalServerError_CODE = "500";     
    public const string InvalidDateRange_CODE = "400";        
    public const string InvalidDownloadToken_CODE = "INVALID_DOWNLOAD_TOKEN";
    public const string HealthcareError_CODE = "500";
    public const string InvalidNoteLength_CODE = "Invalid note length!";
    public const string ProtocolUpdate_CODE = "ProtocolNotFound";
    
}
