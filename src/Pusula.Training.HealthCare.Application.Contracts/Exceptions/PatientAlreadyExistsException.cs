using Volo.Abp;

namespace Pusula.Training.HealthCare.Exceptions
{
    public class PatientAlreadyExistsException : BusinessException
    {
        public PatientAlreadyExistsException()
            : base("PatientAlreadyExists", "A patient with the same Identity Number or Passport Number already exists.")
        {
        }
    }
}