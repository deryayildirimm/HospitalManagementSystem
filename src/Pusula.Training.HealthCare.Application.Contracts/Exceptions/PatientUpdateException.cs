using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions
{
    //Created by Anıl Oğuzman
    public class PatientUpdateException() : BusinessException("PatientUpdate",
        "An error occurred while registering the patient. Please try again, or contact support if the issue persists.")
    {
    }
}