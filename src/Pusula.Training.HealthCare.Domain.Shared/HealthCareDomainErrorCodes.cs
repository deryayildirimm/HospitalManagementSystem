namespace Pusula.Training.HealthCare;

public static class HealthCareDomainErrorCodes
{
   
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
